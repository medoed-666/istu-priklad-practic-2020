using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Task_NetServer
{
    class Program
    {
        static Random random = new Random();

        static string[] CommandNames = new string[]
        {
            "add",
            "range",
            "rand",
            "copy",
            "clear",
            "pop",
            "mul",
            "neg",
            "abs",
            "print",
            "top",
            "count",
            "countodd",
            "counteven",
            "sum",
            "sumodd",
            "sumeven",
            "avg",
        };

        const int COMMAND_ADD = 1;
        const int COMMAND_RANGE = 2;
        const int COMMAND_MUL = 7;

        static byte[] ReadSelectedCommands(BinaryReader reader, int maxCommands)
        {
            byte[] selectedCommands = new byte[maxCommands];
            int readedSelectedCommands = 0;
            while (readedSelectedCommands < selectedCommands.Length)
            {
                int input = reader.ReadByte();
                if (input == -1) break;
                selectedCommands[readedSelectedCommands] = (byte)input;
                readedSelectedCommands++;
            }
            return selectedCommands;
        }

        static byte ChooseRandomCommand(int iteration, IList<byte> selectedCommands, IList<byte> selectedAddingCommands)
        {
            if (iteration <= 5 && selectedAddingCommands.Count > 0)
            {
                return selectedAddingCommands[random.Next(selectedAddingCommands.Count)];
            }
            else
            {
                return selectedCommands[random.Next(selectedCommands.Count)];
            }
        }

        static void Main(string[] args)
        {
            int port = 3005;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                {
                    Console.WriteLine($"ОШИБКА: неверный формат порта {args[0]}");
                }
            }

            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                
                server.Start();
                Console.WriteLine($"Сервер запущен на порту {port}");

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Ожидание подключений... ");
                        Console.WriteLine();

                        // получаем входящее подключение
                        using (TcpClient client = server.AcceptTcpClient())
                        using (NetworkStream stream = client.GetStream())
                        {
                            var reader = new BinaryReader(stream);
                            var writer = new BinaryWriter(stream);
                            Console.WriteLine("Клиент подключен. Ожидание перечня команд...");
                            Console.WriteLine();

                            var selectedCommands = ReadSelectedCommands(reader, 5);
                            var selectedAddingCommands = new List<byte>();
                            Console.WriteLine("Получены команды:");
                            for (int i = 0; i < selectedCommands.Length; i++)
                            {
                                Console.Write($"{i + 1}) ");
                                if (selectedCommands[i] < 1) throw new Exception("Полученная команда должна быть больще или равна 1");
                                if (selectedCommands[i] > CommandNames.Length) throw new Exception("Полученная команда не существует");
                                Console.WriteLine(CommandNames[selectedCommands[i] - 1]);

                                if (selectedCommands[i] <= 4) selectedAddingCommands.Add(selectedCommands[i]);
                            }
                            Console.WriteLine("");

                            int commandCount = random.Next(10, 20);
                            Console.WriteLine($"Отправляю команды ({commandCount}):");
                            writer.Write(commandCount);
                            for (int i = 0; i < commandCount; i++)
                            {
                                Console.Write($"{i + 1}) ");

                                byte command = ChooseRandomCommand(i, selectedCommands, selectedAddingCommands);
                                writer.Write(command);
                                Console.Write(CommandNames[command - 1]);

                                switch (command)
                                {
                                    case COMMAND_ADD:
                                    {
                                        int number = random.Next(-100, 101);
                                        writer.Write(number);
                                        Console.Write($" {number}");
                                        break;
                                    }
                                    case COMMAND_RANGE:
                                    {
                                        int number = random.Next(-100, 101);
                                        int range = random.Next(2, 5);
                                        writer.Write(number);
                                        writer.Write(number + range);
                                        Console.Write($" {number} {number + range}");
                                        break;
                                    }
                                    case COMMAND_MUL:
                                    {
                                        int number = random.Next(-4, 5);
                                        writer.Write(number);
                                        Console.Write($" {number}");
                                        break;
                                    }
                                }
                                
                                Console.WriteLine();
                            }
                            Console.WriteLine();

                            Console.WriteLine($"Все сделано! Отключаюсь");
                            Console.WriteLine();
                        }                            

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ОШИБКА: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ОШИБКА: {e.Message}");
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
            Console.ReadKey();
        }
    }
}

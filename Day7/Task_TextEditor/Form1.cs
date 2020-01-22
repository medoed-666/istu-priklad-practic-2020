using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_TextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox.Text = getRandomInputText();
        }

        private string getRandomInputText()
        {
            string[] variants =
            {
                "В томленьях грусти безнадежной\r\nВ тревогах шумной суеты,\r\nЗвучал мне долго голос нежный\r\nИ снились милые черты.",
                "Сижу за решеткой в темнице сырой.\r\nВскормленный в неволе орел молодой,\r\nМой грустный товарищ, махая крылом,\r\nКровавую пищу клюет под окном,",
                "Я памятник себе воздвиг нерукотворный,\r\nК нему не зарастет народная тропа,\r\nВознесся выше он главою непокорной\r\nАлександрийского столпа.",
                "Я помню чудное мгновенье:\r\nПередо мной явилась ты,\r\nКак мимолетное виденье,\r\nКак гений чистой красоты.",
                "Мороз и солнце; день чудесный!\r\nЕще ты дремлешь, друг прелестный —\r\nПора, красавица, проснись:\r\nОткрой сомкнуты негой взоры\r\nНавстречу северной Авроры,\r\nЗвездою севера явись!",
                "У лукоморья дуб зеленый;\r\nЗлатая цепь на дубе том:\r\nИ днем и ночью кот ученый\r\nВсё ходит по цепи кругом;\r\nИдет направо — песнь заводит,\r\nНалево — сказку говорит.",
                "Подруга дней моих суровых,\r\nГолубка дряхлая моя!\r\nОдна в глуши лесов сосновых\r\nДавно, давно ты ждешь меня.\r\nТы под окном своей светлицы\r\nГорюешь, будто на часах,\r\nИ медлят поминутно спицы\r\nВ твоих наморщенных руках.\r\nГлядишь в забытые вороты\r\nНа черный отдаленный путь;\r\nТоска, предчувствия, заботы\r\nТеснят твою всечасно грудь.\r\nТо чудится тебе . . . . . . . . .",
                "Унылая пора! Очей очарованье!\r\nПриятна мне твоя прощальная краса —\r\nЛюблю я пышное природы увяданье,\r\nВ багрец и в золото одетые леса,\r\nВ их сенях ветра шум и свежее дыханье,\r\nИ мглой волнистою покрыты небеса,\r\nИ редкий солнца луч, и первые морозы,\r\nИ отдаленные седой зимы угрозы.",
                "Буря мглою небо кроет,\r\nВихри снежные крутя;\r\nТо, как зверь, она завоет,\r\nТо заплачет, как дитя,\r\nТо по кровле обветшалой\r\nВдруг соломой зашумит,\r\nТо, как путник запоздалый,\r\nК нам в окошко застучит.\r\nНаша ветхая лачужка\r\nИ печальна и темна.\r\nЧто же ты, моя старушка,\r\nПриумолкла у окна?\r\nИли бури завываньем\r\nТы, мой друг, утомлена,\r\nИли дремлешь под жужжаньем\r\nСвоего веретена?",
                "Я вас любил: любовь еще, быть может,\r\nВ душе моей угасла не совсем;\r\nНо пусть она вас больше не тревожит;\r\nЯ не хочу печалить вас ничем.\r\nЯ вас любил безмолвно, безнадежно,\r\nТо робостью, то ревностью томим;\r\nЯ вас любил так искренно, так нежно,\r\nКак дай вам бог любимой быть другим.",
                "Здесь Пушкин погребен; он с музой молодою,\r\nС любовью, леностью провел веселый век,\r\nНе делал доброго, однако ж был душою,\r\nЕй-богу, добрый человек."
            };

            var rand = new Random();
            return variants[rand.Next(variants.Length)];
        }

        private void applyPlugin_Click(object sender, EventArgs e)
        {
            var open_dialog = new OpenFileDialog();
            open_dialog.Filter = "Плагины (*.dll)|*.dll";
            var open_dialog_res = open_dialog.ShowDialog();
            if (open_dialog_res == DialogResult.OK)
            {
                try
                {
                    if (!File.Exists(open_dialog.FileName))
                    {
                        throw new Exception("Файл не найден");
                    }

                    Assembly DLL = Assembly.LoadFrom(open_dialog.FileName);
                    Type classType = DLL.GetType("Task_TextEditor.Plugin");
                    if (classType == null)
                    {
                        throw new Exception("Класс Plugin в dll не найден");
                    }

                    MethodInfo methodInfo = classType.GetMethod("Execute");
                    if (methodInfo == null)
                    {
                        throw new Exception("Метод Execute в классе Plugin не найден");
                    }

                    var inst = Activator.CreateInstance(classType);
                    object result = methodInfo.Invoke(inst, new object[] { textBox.Text });
                    textBox.Text = result as string;

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }
    }
}

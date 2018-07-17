using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seti_ftp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public string cut_to_point(string input)
        {
            for (int i = input.Length; i >= 1; i--)
            {
                if (input[i - 1] == '.')
                {
                    input = input.Remove(0, i);
                    break;
                }
            }
            return input;
        }
        string struct_dirs(ref Types[] types, Client session, string path, string tree)
        {
            //session.CDUP();
            string buffer = null;
            session.CWD(path);
            foreach (string dir in session.ListDirectory())
            {
                if (!dir.Contains("."))
                {
                    buffer += tree + dir + "\n";
                    string buf = session.uri; //здесь происходит волшебство, зря потрачено 5 часов
                    buffer += struct_dirs(ref types, session, "/" + dir.Substring(dir.IndexOf("/") + 1), tree + "--");
                    session.uri = buf;
                }
                else
                {
                    buffer += tree + dir.Substring(dir.IndexOf("/") + 1) + "\n";
                    bool fined = false;
                    for (int i = 0; i < types.Length; i++)
                    {
                        if (types[i].type == cut_to_point(dir))
                        {
                            fined = true;
                            types[i].size += session.GetFileSize(session.uri + "/" + dir.Substring(dir.IndexOf("/") + 1));
                            break;
                        }
                    }
                    if (!fined)
                    {
                        Array.Resize<Types>(ref types, types.Length + 1);
                        types[types.Length - 1] = new Types(cut_to_point(dir), session.GetFileSize(session.uri + "/" + dir.Substring(dir.IndexOf("/") + 1)));
                    }
                }
            }
            return buffer;
        }
        string sizes(Types[] types)
        {
            string buffer = null;
            foreach(Types elem in types)
            {
                string[] names = {"B", "KB", "MB", "GB", "TB", "PB"};
                int count = 0;
                while (elem.size / 1024 > 0)
                {
                    count++;
                    elem.size /= 1024;
                }
                buffer += elem.type + ": " + elem.size + names[count] + "\n";
            }
            return buffer;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            if (!((textBox1.Text == "Сервер") || (textBox2.Text == "Логин") || (textBox3.Text == "Пароль")))
            {
                textBox4.Text = "Программа написана в рамках третьей ЛР по курсу Сети ЭВМ 2016 год СГАУ. Смирнов Артем ";
                Types[] types = new Types[1];
                types[0] = new Types("all",0);
                Client session = new Client(textBox1.Text, textBox2.Text, textBox3.Text);
                richTextBox2.Text += struct_dirs(ref types, session, "", "");
                richTextBox1.Text += sizes(types);
            }
            else textBox4.Text = "Ошибка! Введите параметры подключения!";
        }
    }
}

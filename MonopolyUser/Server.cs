using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    public partial class ServerForm : Form
    {
        IServerMessanger S1 = new ServerMessanger();
        public ServerForm()
        {
            InitializeComponent();
            S1.NewMessage += S1_NewMessage;
            S1.ClientDisconnect += S1_ClientDisconnect;
            S1.UserConnected += S1_ClientConnect;
        }

        private void S1_ClientConnect(string obj)
        {
            Action Log = () => listBox1.Items.Add(obj + " has been connected\n");
            Invoke(Log);
            Action AddName = () => comboBox1.Items.Add(obj);
            Invoke(AddName);
        }

        private void S1_ClientDisconnect(string obj)
        {
            MessageBox.Show(string.Format("User {0} has been disconected", obj));
            Action Log = () => listBox1.Items.Add(obj + " has been disconnected\n");
            Invoke(Log);
            Action Clear = () => comboBox1.Items.Remove("User " + obj);
            Invoke(Clear);
        }

        private void S1_NewMessage(string obj)
        {
            Action Log = () => listBox1.Items.Add(obj);
            Invoke(Log);
            //тут обработка сообщения
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Server is ready!\n waiting users");
            new Thread(S1.UDPShown).Start();
            new Thread(S1.StartSRV).Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Start game");
            S1.ListenAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            S1.SendTo((byte)comboBox1.SelectedIndex, textBox1.Text);
        }
    }
}

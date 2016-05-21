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

namespace User
{
    public partial class UserForm : Form
    {
        IUserMessanger user = new UserMessanger();
        IUserMessangConverter userConvert = new UserMsgConverter();
        List<Thread> Threads = new List<Thread>(3);
        public UserForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            user.NewMsg += User_NewMsg;
            user.NewUDPMsg += User_NewMsg;
            user.Error += User_Error;
            userConvert.AuctionStart += UserConvert_AuctionStart;
            userConvert.OwnerEP += UserConvert_OwnerEP;
            userConvert.SellStrit += UserConvert_SellStrit;
            userConvert.SystemMsg += UserConvert_SystemMsg;
            userConvert.UpdateDeposit += UserConvert_UpdateDeposit;
            userConvert.UpdateStrits += UserConvert_UpdateStrits;
            userConvert.Error += UserConvert_Error;
        }

        private void UserConvert_Error(string obj)
        {
            MessageBox.Show(obj);
        }

        private void User_Error(string obj)
        {
            MessageBox.Show(obj);
        }

        private void UserConvert_UpdateStrits(byte[] obj)
        {
            Action act = () =>
            {
                listBox2.Items.Clear();
                foreach (var a in obj)
                    listBox2.Items.Add(a);
            };
            this.Invoke(act);
        }

        private void UserConvert_UpdateDeposit(int arg1, string arg2)
        {
            MessageBox.Show(arg2);
            Action act = () => label1.Text = arg1.ToString();
            this.Invoke(act);
        }

        private void UserConvert_SystemMsg(string obj)
        {
            MessageBox.Show(obj);
        }

        private void UserConvert_SellStrit(int arg1, byte arg2, string arg3)
        {
            if (MessageBox.Show(string.Format(" Предложение от: {0}\n Предложенная цена: {1}\n За улицу: {2}", arg3, arg1, arg2), "Предложена сделака:", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK)
                //тут надо написать отрпавку сообщения о продаже серверу
                MessageBox.Show("продано!");
        }

        private void UserConvert_OwnerEP(string obj)
        {
            throw new NotImplementedException();
        }

        private void UserConvert_AuctionStart(byte obj)
        {
            ///я хз как делать аукцион) по идее это должно быть подприложение (окно) где делают ставки...
            throw new NotImplementedException();
        }

        private void User_NewMsg(string obj)
        {
            Action act = () => listBox1.Items.Add(obj);
            this.Invoke(act);
            userConvert.Parse(obj);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                user.FindServer();
                if ((user as UserMessanger).ServerIP != null)
                {
                    MessageBox.Show(string.Format("Найден сервер: {0}", (user as UserMessanger).ServerIP));
                    user.ConnectToSRV(textBox1.Text);
                    Thread th = new Thread(user.ListenTCP);
                    Thread th2 = new Thread(user.ListenUDP);
                    Threads.Add(th);
                    Threads.Add(th2);
                    th.IsBackground = true;
                    th2.IsBackground = true;
                    th.Start();
                    th2.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}

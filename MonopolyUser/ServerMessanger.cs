using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class ServerMessanger : IServerMessanger
    {
        Socket ListenUDP;
        Socket ListenTCP;
        Socket[] Client = new Socket[8];
        List<Client> Clients = new List<Client>();
        public volatile bool _shold_wait_users = true;

        public ServerMessanger(int port=314)
        {
            this.port = port;
            ListenUDP = new Socket(AddressFamily.InterNetwork,
    SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
            ListenUDP.Bind(iep);
            ListenTCP = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
            ListenTCP.Bind(iep);
            Msgs = new List<string>();
        }

        //interface

        public List<string> Msgs
        {
            get;
        }

        public int port
        {
            get;
            private set;
        }

        public event Action<string> ClientDisconnect;
        public event Action<string> NewMessage;
        public event Action<string> UserConnected;

        /// <summary>
        /// Подписываемся на сообщения от всех клиентов
        /// </summary>
        public void ListenAll()
        {
            //перестаем ожидать подключений
            _shold_wait_users = false;
            //подписываемся на сообщения
            try
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    //отписываемся
                    Clients[i].HaveMessage -= _client_message;
                    //подписываемся
                    Clients[i].HaveMessage += _client_message;
                    Thread th = new Thread(Clients[i].Recive);
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region обработчики событий
        private void SRV_ClientDisconect(Client sender)
        {
            Clients.Remove(sender);
            ClientDisconnect?.Invoke(sender.Name);
        }

        private void _client_message(string msg)
        {
            if (msg != null)
            {
                Msgs.Add(msg);
                NewMessage?.Invoke(msg);
            }
        }
        #endregion

        public void SendTo(byte ID, string msg)
        {
            Clients[ID].Send(msg);
        }


        public void StartSRV()
        {
            try
            {
                ListenTCP.Listen(8);
                int i = 0;
                while (_shold_wait_users)
                {
                    var temp = ListenTCP.Accept();
                    byte[] bufer = new byte[1024];
                    int size = temp.Receive(bufer);
                    Clients.Add(new Client(temp, Encoding.ASCII.GetString(bufer, 0, size)));
                    UserConnected(string.Format("User {0}", Clients.Last().Name));
                    //подписываемся
                    Clients.Last().ClientDisconect += SRV_ClientDisconect;
                    Clients.Last().HaveMessage += _client_message;
                    //начинаем слушать
                    Thread th = new Thread(Clients.Last().Recive);
                    th.Start();
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// делает сервер видимым в сети
        /// </summary>
        public void UDPShown()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);

            EndPoint ep = (EndPoint)iep;
            try
            {
                //разрешаем подключения
                while (true)
                {
                    //слушаем
                    byte[] data = new byte[1024];
                    //ListenUDP.ReceiveTimeout=100000;
                    int recv = ListenUDP.ReceiveFrom(data, ref ep);
                    string stringData = Encoding.ASCII.GetString(data, 0, recv);
                    //отвечаем
                    IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ep.ToString().Split(':')[0]), int.Parse(ep.ToString().Split(':')[1]));
                    byte[] msg = Encoding.ASCII.GetBytes(ipe.ToString());
                    ListenUDP.SendTo(msg, ipe);
                }
            }
            catch
            {
                UDPShown();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public Socket MainSocket;
        public string Name { get; private set; }

        /// <summary>
        /// бесконечно слушаем сокет и генерируем события
        /// </summary>
        public void Recive()
        {
            try
            {
                while (true)
                {
                    byte[] bufer = new byte[1024];
                    int bytesRec = MainSocket.Receive(bufer);
                    if (HaveMessage != null)
                        HaveMessage(Encoding.ASCII.GetString(bufer, 0, bytesRec) + string.Format(" From User {0} IP {1} ;", Name, MainSocket.RemoteEndPoint));
                }
            }
            catch (Exception)
            {
                if (ClientDisconect != null)
                    ClientDisconect(this);
            }
        }

        /// <summary>
        /// отправляем сообщение
        /// </summary>
        public void Send(string msg)
        {
            byte[] bufer = Encoding.ASCII.GetBytes(msg);
            MainSocket.Send(bufer);
        }

        public Client(Socket ConnectedSocked, string UserName = "Ivan")
        {
            Name = UserName;
            MainSocket = ConnectedSocked;
        }

        public event Action<string> HaveMessage;
        public event Action<Client> ClientDisconect;
    }
}

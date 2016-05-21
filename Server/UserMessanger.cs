using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace User
{
    class UserMessanger : IUserMessanger
    {
        #region события
        public event Action<string> NewMsg;
        public event Action<string> NewUDPMsg;
        public event Action<string> Error;
        #endregion

        #region поля
        int ServerPort = 314;
        public IPAddress ServerIP { get; private set; }
        IPEndPoint otherClient;

        Socket SRV;
        Socket UDP;
        #endregion

        #region методы
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public UserMessanger()
        {
            UDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                ProtocolType.Udp);
            SRV = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
        }

        /// <summary>
        /// установить соединение с сервером
        /// </summary>
        /// <param name="yourName">Имя пользователя</param>
        public void ConnectToSRV(string yourNick)
        {
            try
            {
                SRV.Connect(ServerIP, ServerPort);
                SRV.Send(Encoding.ASCII.GetBytes(yourNick));
            }
            catch
            {
                Error?.Invoke("Can't connect to srv, please find other server");
            }
        }

        /// <summary>
        /// Найти сервер
        /// </summary>
        public void FindServer()
        {
            try
            {
                //кричим в сеть
                Socket MySock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                    ProtocolType.Udp);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Broadcast, ServerPort);
                byte[] msg = Encoding.ASCII.GetBytes("1");
                MySock.SetSocketOption(SocketOptionLevel.Socket,
                    SocketOptionName.Broadcast, 1);
                MySock.SendTo(msg, ipe);
                //слушаем ответы
                EndPoint ep = (EndPoint)ipe;
                byte[] data = new byte[1024];
                MySock.ReceiveTimeout = 15;
                int recv = MySock.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                //записываем адрес сервера
                ServerIP = ((IPEndPoint)ep).Address;
                SRV.Bind(MySock.LocalEndPoint);
                MySock.Close();
            }
            catch
            {
                Error?.Invoke("There is no server in your local net.");
            }
        }


        /// <summary>
        /// Включает прослушивание TCP канала порта
        /// </summary>
        public void ListenTCP()
        {

            try
            {
                while (true)
                {
                    byte[] bufer = new byte[1024];
                    int bytesRec = SRV.Receive(bufer);
                    NewMsg?.Invoke(Encoding.ASCII.GetString(bufer, 0, bytesRec));
                }
            }
            catch (Exception)
            {
                Error?.Invoke("Сервер отключился");
            }
        }


        /// <summary>
        /// включает прослушивание UDP канала порта
        /// </summary>
        public void ListenUDP()
        {
            try
            {
                EndPoint ep = SRV.LocalEndPoint;
                UDP.Bind(ep);
                while (true)
                {
                    //слушаем
                    byte[] data = new byte[1024];
                    int recv = UDP.ReceiveFrom(data, ref ep);
                    string stringData = Encoding.ASCII.GetString(data, 0, recv);
                    NewMsg(stringData);
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex.Message);
            }
        }

        /// <summary>
        /// отправить сообщение на сервер
        /// </summary>
        /// <param name="msg">Сообщение</param>
        public void SendToSRV(string msg)
        {
            try
            {
                byte[] bmsg = Encoding.ASCII.GetBytes(msg);
                SRV.Send(bmsg);
            }
            catch
            {
                Error?.Invoke("Can't send message, please try to reconnect to your server");
            }
        }

        /// <summary>
        /// отправляет сообщение другому пользователю (клиенту)
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="socket">Адрес пользователя (IP:Port)</param>
        public void UDPSend(string msg, string socket = null)
        {

            byte[] byteMSG = Encoding.ASCII.GetBytes(msg);
            if (socket == null)
            {
                UDP.SendTo(byteMSG, otherClient);
            }
            else
            {
                IPEndPoint r = new IPEndPoint(IPAddress.Parse(socket.Split(':')[0]), int.Parse(socket.Split(':')[1]));
                UDP.SendTo(byteMSG, r);
            }
        }
        #endregion

        public void Disconnect()
        {
            NewMsg = null;
            NewUDPMsg = null;
            Error = null;
        UDP.Close();
            SRV.Close();
        }
    }
}

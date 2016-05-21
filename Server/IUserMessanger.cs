using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User
{
    interface IUserMessanger
    {
        void FindServer();
        void ConnectToSRV(string yourNick);
        void SendToSRV(string msg);
        void UDPSend(string msg, string socket);
        void ListenTCP();
        void ListenUDP();

        event Action<string> NewMsg;
        event Action<string> NewUDPMsg;
        event Action<string> Error;
    }
}

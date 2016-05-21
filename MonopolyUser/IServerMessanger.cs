using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    interface IServerMessanger
    {
        int port { get;}
        List<string> Msgs { get; }

        void SendTo(byte ID, string msg);
        void ListenAll();
        void StartSRV();
        void UDPShown();


        event Action<string> NewMessage;
        event Action<string> ClientDisconnect;
        event Action<string> UserConnected;

    }
}

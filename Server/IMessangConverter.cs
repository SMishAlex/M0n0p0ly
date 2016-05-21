using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User
{
    interface IUserMessangConverter
    {
        string Convert(object[] args);
        void Parse(string msg);

        event Action<int, string> UpdateDeposit;
        event Action<byte[]> UpdateStrits;
        event Action<string> SystemMsg;
        event Action<int, byte, string> SellStrit;
        event Action<byte> AuctionStart;
        event Action<string> OwnerEP;
        event Action<string> Error;
    }
}

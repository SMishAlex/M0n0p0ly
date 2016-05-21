using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User
{
    class UserMsgConverter : IUserMessangConverter
    {
        ///Структура входящих запросов: Тип, значения

        /// <summary>
        /// Типы входящих сообщений
        /// </summary>
        public enum InMsgType
        {
            /// <summary>
            /// Системное сообщение
            /// </summary>
            SystemMsg,
            /// <summary>
            /// Обновление баланса
            /// </summary>
            DepositUpdate,
            /// <summary>
            /// обновление недвижимости
            /// </summary>
            StritsUpdate,
            /// <summary>
            /// Запрос на покупку
            /// </summary>
            AskSelling,
            /// <summary>
            /// Начат аукцион
            /// </summary>
            AuctionStart,
            /// <summary>
            /// запрошенная улица имеет другого владельца
            /// </summary>
            OtherOwner
        }

        /// <summary>
        /// Типа исходящих сообщений
        /// </summary>
        public enum OutMsgType
        {
            /// <summary>
            /// Обновление баланса
            /// </summary>
            DepositUpdate = 1,
            /// <summary>
            /// обновление недвижимости
            /// </summary>
            StritsUpdate,
            /// <summary>
            /// Запрос на покупку
            /// </summary>
            AskSelling,
            /// <summary>
            /// Ставка на аукцион
            /// </summary>
            Auction,
            /// <summary>
            /// оплата ренты
            /// </summary>
            Rent,
            /// <summary>
            /// подтверждение сделки (запрос на обмен)
            /// </summary>
            acceptDeal,
            /// <summary>
            /// заложить улицу
            /// </summary>
            establish
        }

        #region События
        /// <summary>
        /// Входящий запрос на продажу улицы
        /// </summary>
        /// <remarks>предлагаемая сумма, улица, имя покупателя</remarks>
        public event Action<int, byte, string> SellStrit;
        /// <summary>
        /// входящее системное сообщение
        /// </summary>
        public event Action<string> SystemMsg;
        /// <summary>
        /// Входящий запрос на обновление баланса
        /// </summary>
        /// <remarks>Новое значение счета, причина обнавления</remarks>
        public event Action<int, string> UpdateDeposit;
        /// <summary>
        /// входящий запрос на обновление улиц принадлежащих игроку
        /// </summary>
        /// <remarks>передается весь список принадлежащих улиц</remarks>
        public event Action<byte[]> UpdateStrits;
        /// <summary>
        /// начало аукциона
        /// </summary>
        /// <remarks>Передается ключ выставленной улицы</remarks>
        public event Action<byte> AuctionStart;
        /// <summary>
        /// запрашиваемая улица принадлежит другому игроку
        /// </summary>
        public event Action<string> OwnerEP;
        public event Action<string> Error;
        #endregion

        /// <summary>
        /// создает строковое сообщение интерпритирующее запрос
        /// </summary>
        /// <param name="args">Первым идет тип запроса а затем аргументя запроса в установленном порядке</param>
        /// <returns></returns>
        public string Convert(object[] args)
        {
            string res = "";
            foreach (var s in args)
            {
                res += s.ToString() + ':';
            }
            return res;
        }

        /// <summary>
        /// Инициализирует события соответствующие пришедшему сообщению
        /// </summary>
        /// <param name="msg">Входящие сообщение</param>
        public void Parse(string msg)
        {
            try
            {
                string[] Args = msg.Split(':');
                switch (byte.Parse(Args[0]))
                {
                    case ((byte)InMsgType.SystemMsg):
                        SystemMsg(Args[1]); break;
                    case ((byte)InMsgType.DepositUpdate):
                        UpdateDeposit(int.Parse(Args[1]), Args[2]); break;
                    case ((byte)InMsgType.StritsUpdate):
                        byte[] strits = new byte[Args.Length - 1];
                        for (int i = 1; i < Args.Length; i++)
                        {
                            strits[i - 1] = byte.Parse(Args[i]);
                        }
                        UpdateStrits(strits);
                        break;
                    case ((byte)InMsgType.AskSelling):
                        SellStrit(int.Parse(Args[1]), byte.Parse(Args[2]), Args[3]);
                        break;
                    case ((byte)InMsgType.AuctionStart):
                        AuctionStart(byte.Parse(Args[1]));
                        break;
                    case ((byte)InMsgType.OtherOwner):
                        OwnerEP(Args[1] + ':' + Args[2]);
                        break;
                    default:
                        throw new Exception("Не известный тип входящего сообщения");
                }
            }
            catch(Exception ex)
            {
                Error?.Invoke(ex.Message);
            }
        }
    }
}

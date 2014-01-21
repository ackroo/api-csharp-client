using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ackroo.Utils.Json
{
    public class Transaction 
    {
        public string Transaction_Number { get; set; }
        public string Transaction_Date_Time { get; set; }
        public string Gc_Balance { get; set; }
        public string El_Balance { get; set; }
        public string Amount_Funded { get; set; }
        public string Amount_Redeemed { get; set; }
    }

    public class Card
    {
        public string Gift { get; set; }
        public string Loyalty { get; set; }
    }

    public class Error
    {
        public string error { get; set; }
    }


}

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

    public class OAuthCode
    {
        public string user_code { get; set; }
        public string device_code { get; set; }
        public int expires_in { get; set; }
        public string verification_url { get; set; }
        public int interval { get; set; }
    }

    public class OAuthToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        //public int expires_in { get; set; } handle null value
        public string token_type { get; set; }
        public string scope { get; set; }
        public string vendor { get; set; }
    }

    public class OAuthTokenError
    {
        public string error { get; set; }
        public string error_description { get; set; }
    }

}

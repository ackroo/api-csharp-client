using System;
using System.Net;
using Newtonsoft.Json;


namespace Ackroo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string terminal = args[0];
                string card = args[1];

                Console.WriteLine("Checking card balance\n");
                card_balance(terminal, card);

                Console.WriteLine("Fund gift for $20\n");
                fund_gift_transaction(terminal, card, 20);

                Console.WriteLine("Redeem gift for $10\n");
                redeem_gift_transaction(terminal, card, 10);

                Console.WriteLine("Fund loyalty for $1000\n");
                fund_loyalty_transaction(terminal, card, 1000);
              
                Console.WriteLine("Redeem loyalty for $30\n");
                redeem_loyalty_transaction(terminal, card, 30);

                Console.WriteLine("Done !!!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }

        }

        public static void card_balance(string terminal, string card)
        {
            string transactionRequest = Ackroo.Utils.Http.Client.CreateRequest("/terminal/card/balance");
            Ackroo.Utils.Json.Card cardResponse = MakeRequest(transactionRequest+build_cardholder_query(card, terminal));
        }

        public static void fund_gift_transaction(string terminal, string card, int amount)
        {
            string transactionRequest = Ackroo.Utils.Http.Client.CreateRequest("/transaction/gift/credit");
            Ackroo.Utils.Json.Transaction transactionResponse = MakeRequest(transactionRequest, build_transaction_query(terminal, card, amount));
        }

        public static void redeem_gift_transaction(string terminal, string card, int amount)
        {
            string transactionRequest = Ackroo.Utils.Http.Client.CreateRequest("/transaction/gift/debit");
            Ackroo.Utils.Json.Transaction transactionResponse = MakeRequest(transactionRequest, build_transaction_query(terminal, card, amount));
        }

        public static void fund_loyalty_transaction(string terminal, string card, int amount)
        {
            string transactionRequest = Ackroo.Utils.Http.Client.CreateRequest("/transaction/loyalty/credit");
            Ackroo.Utils.Json.Transaction transactionResponse = MakeRequest(transactionRequest, build_transaction_query(terminal, card, amount));
        }

        public static void redeem_loyalty_transaction(string terminal, string card, int amount)
        {
            string transactionRequest = Ackroo.Utils.Http.Client.CreateRequest("/transaction/loyalty/debit");
            Ackroo.Utils.Json.Transaction transactionResponse = MakeRequest(transactionRequest, build_transaction_query(terminal, card, amount));
        }

        private static T _download_serialized_json_data<T>(string json_data) where T : new()
        {
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

        public static string build_transaction_query(string terminal, string card, int amount)
        {
            string query = "terminal_serial_number=" + terminal + "&cardnumber=" + card + "&amount=" + amount;
            return query;
        }

        public static string build_cardholder_query(string card, string terminal)
        {
            string query = "?cardnumber=" + card + "&terminal_serial_number=" + terminal;
            return query;
        }

        public static Ackroo.Utils.Json.Transaction MakeRequest(string requestUrl, string query)
        {
            try
            {
                string response = Ackroo.Utils.Http.Client.HttpPost(requestUrl, query);
                Console.WriteLine(response + "\n");
                Ackroo.Utils.Json.Transaction transaction = _download_serialized_json_data<Ackroo.Utils.Json.Transaction>(response);
                return transaction;            
            }
            catch (Exception e)
            {
                Ackroo.Utils.Json.Error error = _download_serialized_json_data<Ackroo.Utils.Json.Error>(e.Message);
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public static Ackroo.Utils.Json.Card MakeRequest(string requestUrl)
        {
            try
            {
                string response = Ackroo.Utils.Http.Client.HttpGet(requestUrl);
                Console.WriteLine(response + "\n");
                Ackroo.Utils.Json.Card card = _download_serialized_json_data<Ackroo.Utils.Json.Card>(response);
                return card;
            }
            catch (Exception e)
            {
                Ackroo.Utils.Json.Error error = _download_serialized_json_data<Ackroo.Utils.Json.Error>(e.Message);
                Console.WriteLine(e.Message);
                return null;
            }

        }
        
    }
}

using System;
using System.Net;
using Newtonsoft.Json;

namespace Ackroo.Client
{
    static class Constants
    {
        public const string oauth_scopes = "terminal transaction";
        public const string grant_type = "authorization_code";
        public const string client_id = "a22b1ee5fde572f5f3ec289a22621fe92208f4ae2059f9603cc663abcb303d3b";
        public const string client_secret = "d55bba344418d8f4e498334e4838a6651c024466489295421e8cd02213aebea7";
    }

    class Program
    {
        public static string oauth_token = "";
        static void Main(string[] args)
        {
            try
            {
                
                Console.WriteLine("Getting oauth access token\n");
                oauth_flow();

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

        public static void oauth_flow()
        {
            string oAuthCodeRequest = Ackroo.Utils.Http.Client.CreateOAuthRequest("/oauth/devices/code");
            string code_query = "client_id=" + Constants.client_id + "&scope=" + Constants.oauth_scopes;
            try
            {
                string code_response = Ackroo.Utils.Http.Client.HttpPost(oAuthCodeRequest, code_query);
                Console.WriteLine("Debug: " + code_response + "\n");
                Ackroo.Utils.Json.OAuthCode code = _download_serialized_json_data<Ackroo.Utils.Json.OAuthCode>(code_response);
                
                Console.WriteLine("You have been granted a temporary activation PIN by Ackroo that expires in " + code.expires_in/60 + " minutes.\n");
                Console.WriteLine("Please navigate to " + code.verification_url + " from a browser enabled device, and enter the following code to grant authorization:  \n");
                Console.WriteLine(code.user_code + "\n");
                
                string oAuthTokenRequest = Ackroo.Utils.Http.Client.CreateOAuthRequest("/oauth/token");
                string token_query = "code=" + code.user_code + "&client_id=" + Constants.client_id + "&client_secret=" + Constants.client_secret + "&grant_type=" + Constants.grant_type;

                
                do
                {
                    System.Threading.Thread.Sleep(code.interval * 1000);
                    code.expires_in -= code.interval;
                    if (code.expires_in == 0)
                        Console.WriteLine("Activation PIN expired!\n");

                    try
                    {
                        string token_response = Ackroo.Utils.Http.Client.HttpPost(oAuthTokenRequest, token_query);
                        Console.WriteLine("Debug: " + token_response + "\n");
                        Console.WriteLine("Got access token, proceeding ... \n");
                        Ackroo.Utils.Json.OAuthToken token = _download_serialized_json_data<Ackroo.Utils.Json.OAuthToken>(token_response);
                        Program.oauth_token = token.access_token;
                        break;
                    }
                    catch (Exception e)
                    {
                        Ackroo.Utils.Json.OAuthTokenError error = _download_serialized_json_data<Ackroo.Utils.Json.OAuthTokenError>(e.Message);
                        //Console.WriteLine(error.error_description + "\n");
                        Console.WriteLine("Device code pending authorization by merchant ... \n");
                    }
                    
                } while (true);

                return;
            }
            catch (Exception e)
            {
                Ackroo.Utils.Json.Error error = _download_serialized_json_data<Ackroo.Utils.Json.Error>(e.Message);
                Console.WriteLine(e.Message);
                return;
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

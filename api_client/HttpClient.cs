using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Ackroo.Utils.Http
{
    public class Client
    {
        public static string setupHTTPAuthorizationHeader()
        {
            return "Bearer " + Ackroo.Client.Program.oauth_token;
        }

        //Create the request URL
        public static string CreateRequest(string path)
        {
            string UrlRequest = "https://api.ackroo.net" + path;
            return (UrlRequest);
        }

        //Create the request URL
        public static string CreateOAuthRequest(string path)
        {
            string UrlRequest = "https://manage.ackroo.com" + path;
            return (UrlRequest);
        }

        public static string HttpGet(string URI) 
        {
           HttpWebRequest request = WebRequest.Create(URI) as HttpWebRequest;
           
           //ssl
           request.Credentials = CredentialCache.DefaultCredentials;
           //allows for validation of SSL certificates 
           ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

           //oauth token
           request.Headers["Authorization"] = setupHTTPAuthorizationHeader();

           try
           {
               using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
               {
                   System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                   string resp = sr.ReadToEnd().Trim();
                   if (response.StatusCode != HttpStatusCode.OK)
                       throw new Exception(resp);
                   return resp;
               }
           }
           catch (System.Net.WebException ex)
           {
               HttpWebResponse response = ex.Response as HttpWebResponse;
               System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
               string resp = sr.ReadToEnd().Trim();
               throw new Exception(resp);
           }
        }

        public static string HttpPost(string URI, string Parameters)
        {
            HttpWebRequest request = WebRequest.Create(URI) as HttpWebRequest;
           
            //ssl
            request.Credentials = CredentialCache.DefaultCredentials;
            //allows for validation of SSL certificates 
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            //oauth token
            request.Headers["Authorization"] = setupHTTPAuthorizationHeader();

            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            //We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            request.ContentLength = bytes.Length;
            System.IO.Stream os = request.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                    string resp = sr.ReadToEnd().Trim();
                    if ( (response.StatusCode != HttpStatusCode.Created) && (response.StatusCode != HttpStatusCode.OK) )
                        throw new Exception(resp);
                    return resp;
                }
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                string resp = sr.ReadToEnd().Trim();
                throw new Exception(resp);
            }
        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if(sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }

    }
}
using CoreWPF.Utilites;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MlsExclusive.Utilites
{
    public static class MlsServer
    {
        public static string Flats { get; private set; }
        public static string Houses { get; private set; }

        static MlsServer()
        {
            Flats = "";
            Houses = "";
        }

        public static void GetFeeds()
        {
            User user = App.GetUser();
            string login = user.login;
            string pass = user.password;
            string session = "";
            HttpWebRequest httpWebRequest;
            HttpWebResponse httpResponse;
            string[] feeds = new string[2];

            #region Login code
            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/json.php");
            httpWebRequest.Accept = "*/*";
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Host = "mls.kh.ua";
            httpWebRequest.Referer = "https://mls.kh.ua/";
            httpWebRequest.UserAgent = user.user_agent;
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"action\":\"auth_login\",\"data\":{\"login\":\"" + login + "\",\"password\":\"" + pass + "\"}}";

                streamWriter.Write(json);
            }

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();

                session = new Regex("\"session\": \"[\\w\\d]+\"").Match(result).Value;
                session = new Regex("[\\w\\d]+").Matches(session)[1].Value;
            }

            #endregion

            #region Get feed code
            CookieContainer cookieSend = new CookieContainer(1);
            cookieSend.Add(new Cookie("session", session, "/", "mls.kh.ua"));

            for (int i = 1; i < 3; i++)
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/mls-export.php?num=" + i);
                httpWebRequest.CookieContainer = cookieSend;
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Host = "mls.kh.ua";
                httpWebRequest.UserAgent = user.user_agent;
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    feeds[i - 1] = result;
                    //System.IO.File.WriteAllText(@"E:\request" + i + ".txt", result);
                }
            }
            #endregion

            #region Logout code

            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/json.php");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Referer = "https://mls.kh.ua/";
            httpWebRequest.Host = "mls.kh.ua";
            httpWebRequest.UserAgent = user.user_agent;
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"id\":" + login + ",\"session\":\"" + session + "\",\"action\":\"auth_logout\",\"data\":null}";
                streamWriter.Write(json);
            }

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
            }
            #endregion

            Flats = feeds[0];
            Houses = feeds[1];

            UpdateTime.Set(UnixTime.CurrentUnixTimestamp());
        }

    }
}
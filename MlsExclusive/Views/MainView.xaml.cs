using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MlsExclusive.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            string login = "579";
            string pass = "8751";
            string session = "";
            HttpWebRequest httpWebRequest;
            HttpWebResponse httpResponse;

            #region Login code
            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/json.php");
            httpWebRequest.Accept = "*/*";
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Host = "mls.kh.ua";
            httpWebRequest.Referer = "https://mls.kh.ua/";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"action\":\"auth_login\",\"data\":{\"login\":\""+ login + "\",\"password\":\""+ pass +"\"}}";

                streamWriter.Write(json);
            }

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                //System.IO.File.WriteAllText(@"E:\request.txt", result);
                
                session = new Regex("\"session\": \"[\\w\\d]+\"").Match(result).Value;
                session = new Regex("[\\w\\d]+").Matches(session)[1].Value;
                MessageBox.Show(session);
            }

            #endregion

            #region Get feed code
            CookieContainer cookieSend = new CookieContainer(1);
            cookieSend.Add(new Cookie("session", session, "/", "mls.kh.ua"));

            for(int i = 1; i < 3; i++) { 
                httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/mls-export.php?num=" + i);
                httpWebRequest.CookieContainer = cookieSend;
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Host = "mls.kh.ua";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    System.IO.File.WriteAllText(@"E:\request"+ i +".txt", result);
                }
            }
            #endregion

            #region Logout code

            httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mls.kh.ua/json.php");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Referer = "https://mls.kh.ua/";
            httpWebRequest.Host = "mls.kh.ua";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"id\":"+ login +",\"session\":\""+ session +"\",\"action\":\"auth_logout\",\"data\":null}";

                streamWriter.Write(json);
            }

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();

                MessageBox.Show(result);
            }
            #endregion
        }
    }
}

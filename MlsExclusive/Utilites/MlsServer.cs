using CoreWPF.Utilites;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace MlsExclusive.Utilites
{
    /// <summary>
    /// Представляет собой инструменты для получения фидов МЛС.
    /// </summary>
    public static class MlsServer
    {
        /// <summary>
        /// Путь в реестре к данным для сервера.
        /// </summary>
        private const string RegeditPath = "SOFTWARE/Coreman soft/Mls Exclusive";

        /// <summary>
        /// Строка-фид с квартирами.
        /// </summary>
        public static string Flats { get; private set; }

        /// <summary>
        /// Строка-фид с домами.
        /// </summary>
        public static string Houses { get; private set; }

        /// <summary>
        /// Инициализация.
        /// </summary>
        static MlsServer()
        {
            Flats = "";
            Houses = "";
        }

        /// <summary>
        /// Метод пытается подключиться к МЛС серверу; записывает результаты работы в <see cref="Flats"/> и <see cref="Houses"/>.
        /// </summary>
        public static void GetFeeds()
        {
            try
            {
                MlsServer.Flats = "";
                MlsServer.Houses = "";
                User user = MlsServer.GetUser();
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
                    if (result.Contains("error")) throw new ArgumentException(result.Replace("\r\n", "").Replace("{", "").Replace("}", "").Split(':')[1].Replace("\"", ""));
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

                MlsServer.SetUpdateTime(UnixTime.CurrentUnixTimestamp());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка МЛС сервера!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Возвращает время последней выгрузки из МЛС.
        /// </summary>
        /// <returns>Возвращает время последней выгрузки из МЛС в формате <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset GetUpdateTime()
        {
            try
            {
                return UnixTime.ToDateTimeOffset(
                    Convert.ToDouble(RegeditEditor.Read(Registry.CurrentUser, RegeditPath, "value1"))
                    , App.Timezone);
            }
            catch(Exception ex)
            {
                double current_stamp = UnixTime.CurrentUnixTimestamp();
                SetUpdateTime(current_stamp);
                return UnixTime.ToDateTimeOffset(current_stamp, App.Timezone);
            }
        }

        /// <summary>
        /// Устанавливает время последней выгрузки из МЛС.
        /// </summary>
        /// <param name="timestamp">Unix Timestamp со смещением во времени по <see cref="UnixTime.UTC"/>.</param>
        private static void SetUpdateTime(double timestamp)
        {
            //File.WriteAllText(UnixTimestampPath, timestamp.ToString());
            RegeditEditor.Write(Registry.CurrentUser, RegeditPath, "value1", timestamp, true);
        }

        /// <summary>
        /// Получить информацию для авторизации и дальнейшей загрузки фидов
        /// </summary>
        /// <returns>Структура <see cref="User"/> с данными для фидов</returns>
        public static User GetUser()
        {
            try
            {
                string json = Crypt.DecryptString((string)RegeditEditor.Read(Registry.CurrentUser, RegeditPath, "value2"), "vile goblin");
                return User.Deserialize(json);
            }
            catch
            {
                User tmp_user = new User("", "", "");
                SetUser(tmp_user);
                return tmp_user;
            }
            //return JsonConvert.DeserializeObject<User>(json);
            


        }

        /// <summary>
        /// Задает информацию о авторизации для фидов и записывает её в файл конфигурации
        /// </summary>
        /// <param name="user">Структура <see cref="User"/> с данными для фидов</param>
        public static void SetUser(User user)
        {
            /*
            string json = JsonConvert.SerializeObject(user);
            File.WriteAllText(UserPath, json);
            */
            string value = Crypt.EncryptString(User.Serialize(user), "vile goblin");
            RegeditEditor.Write(Registry.CurrentUser, RegeditPath, "value2", value, true);
        }
    }
}
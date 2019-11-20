using CoreWPF.Utilites;
using MlsExclusive.Utilites;
using Newtonsoft.Json;
using Offer;
using System;
using System.IO;
using System.Windows;

namespace MlsExclusive
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Путь к файлу-конфигурации для фидов МЛС
        /// </summary>
        public static string UserPath { get; set; }

        public static TimeSpan Timezone = UnixTime.Local;

        /// <summary>
        /// Задает <see cref="UserPath"/> по умолчанию.
        /// </summary>
        public App()
        {
            OfferCategory.AddNewCategory("пол-дома");
            UserPath = "Data/user.json";
        }

        /// <summary>
        /// Получить информацию для фидов
        /// </summary>
        /// <returns>Структура <see cref="User"/> с данными для фидов</returns>
        public static User GetUser()
        {
            string json = File.ReadAllText(UserPath); 
            return JsonConvert.DeserializeObject<User>(json);
        }

        /// <summary>
        /// Задает информацию для фидов и записывает её в файл конфигурации
        /// </summary>
        /// <param name="user">Структура <see cref="User"/> с данными для фидов</param>
        public static void SetUser(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            File.WriteAllText(UserPath, json);
        }
    }
}

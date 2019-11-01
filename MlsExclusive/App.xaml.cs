using MlsExclusive.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MlsExclusive
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string UserPath { get; set; }

        public App()
        {
            UserPath = "Data/user.json";
        }

        public static User GetUser()
        {
            string json = File.ReadAllText(UserPath); 
            return JsonConvert.DeserializeObject<User>(json);
        }

        public static void SetUser(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            File.WriteAllText(UserPath, json);
        }
    }
}

using CoreWPF.Utilites;
using Offer;
using System;
using System.Threading;
using System.Windows;

namespace MlsExclusive
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Часовой пояс, используемый в приложении; от него опираются все значения дат и времени.
        /// </summary>
        public static TimeSpan Timezone = TimeZoneInfo.Local.BaseUtcOffset.Add(TimeZoneInfo.Local.IsDaylightSavingTime(DateTimeOffset.Now) ? new TimeSpan(1, 0, 0) : new TimeSpan(0, 0, 0));
        public static string AppTitle = "MLS Exclusive";
        
        /// Хранит именованный мьютекс, чтобы сохранить владение им до конца пробега программы
        private static Mutex InstanceCheckMutex;

        /// <summary>
        /// Проверяем, запущено ли приложение
        /// </summary>
        /// <returns>Возвращает true, если приложение не запущено, иначе false</returns>
        private static bool InstanceCheck()
        {
            bool isNew;
            App.InstanceCheckMutex = new Mutex(true, App.AppTitle, out isNew);
            return isNew;
        } //---метод InstanceCheck

        /// <summary>
        /// Добавляет новые значения в <see cref="OfferCategory"/>.
        /// </summary>
        public App()
        {
            if (!App.InstanceCheck())
            {
                MessageBox.Show("Программа уже запущена!", App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Environment.Exit(0);
            }

            OfferCategory.AddNewCategory("пол-дома");
        }
    }
}

using CoreWPF.Utilites;
using Offer;
using System;
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
        public static TimeSpan Timezone = UnixTime.Local;

        /// <summary>
        /// Добавляет новые значения в <see cref="OfferCategory"/>.
        /// </summary>
        public App()
        {
            OfferCategory.AddNewCategory("пол-дома");
        }
    }
}

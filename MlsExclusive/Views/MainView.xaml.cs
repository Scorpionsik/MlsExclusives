using CoreWPF.Windows;
using MlsExclusive.Models;
using MlsExclusive.ViewModels;

namespace MlsExclusive.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowExt
    {
        /// <summary>
        /// Инициализация окна.
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            MainViewModel vm = new MainViewModel();
            vm.Event_ScrollIntoCurrentOffers += new System.Action<MlsOffer>(this.ScrollIntoCurrentOffer);
            vm.Event_ScrollIntoCurrentAgency += new System.Action<Agency>(this.ScrollIntoCurrentAgency);
            this.DataContext = vm;
        }

        private void ScrollIntoCurrentOffer(MlsOffer offer)
        {
            this.CurrentOffers.ScrollIntoView(offer);
        }

        private void ScrollIntoCurrentAgency(Agency agency)
        {
            this.Agencys.ScrollIntoView(agency);
        }
    }
}

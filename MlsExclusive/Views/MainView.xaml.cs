using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using CoreWPF.Windows;
using MlsExclusive.Models;
using MlsExclusive.Utilites.Enums;
using MlsExclusive.ViewModels;
using MlsExclusive.Views.MainModules;

namespace MlsExclusive.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowExt
    {
        public MainView()
        {
            InitializeComponent();
            NavigationManager navigator = new NavigationManager(this.Dispatcher, this.OfferTable);
            navigator.Register<ListExt<MlsOffer>, FlatModule>(new ListExt<MlsOffer>(), MlsMode.Flat.ToString());
            navigator.Register<ListExt<MlsOffer>, HouseModule>(new ListExt<MlsOffer>(), MlsMode.House.ToString());
            this.DataContext = new MainViewModel(navigator);
        }
    }
}

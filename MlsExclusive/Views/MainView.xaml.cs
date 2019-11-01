using CoreWPF.Windows;
using MlsExclusive.ViewModels;

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
           
            this.DataContext = new MainViewModel();
        }
    }
}

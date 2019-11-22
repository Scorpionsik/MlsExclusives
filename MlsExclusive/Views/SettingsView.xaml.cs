using CoreWPF.Windows;
using MlsExclusive.ViewModels;

namespace MlsExclusive.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : DialogWindowExt
    {
        /// <summary>
        /// Инициализация окна.
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }
    }
}

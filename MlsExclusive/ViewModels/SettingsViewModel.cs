using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Utilites;
using MlsExclusive.Views;

namespace MlsExclusive.ViewModels
{
    /// <summary>
    /// Логика для <see cref="SettingsView"/>.
    /// </summary>
    public class SettingsViewModel : ViewModel
    {
        private string login;
        /// <summary>
        /// Поле для логина.
        /// </summary>
        public string Login
        {
            get { return this.login; }
            set
            {
                this.login = value;
                this.OnPropertyChanged("Login");
            }
        }

        private string password;
        /// <summary>
        /// Поле для пароля.
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                this.OnPropertyChanged("Password");
            }
        }

        private string user_agent;
        /// <summary>
        /// Поле для User agent.
        /// </summary>
        public string User_agent
        {
            get { return this.user_agent; }
            set
            {
                this.user_agent = value;
                this.OnPropertyChanged("User_agent");
            }
        }

        /// <summary>
        /// Инициализация логики окна <see cref="SettingsView"/>.
        /// </summary>
        public SettingsViewModel()
        {
            this.Title = "Настройки";
            User current_user = MlsServer.GetUser();

            this.login = current_user.login;
            this.password = current_user.password;
            this.user_agent = current_user.user_agent;
        }

        /// <summary>
        /// Команда для сохранения внесённых изменений из полей текущего окна в объект <see cref="User"/>.
        /// </summary>
        public override RelayCommand Command_save
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    MlsServer.SetUser(new User(this.Login, this.Password, this.User_agent));
                    base.Command_save?.Execute();
                });
            }
        }
    }
}

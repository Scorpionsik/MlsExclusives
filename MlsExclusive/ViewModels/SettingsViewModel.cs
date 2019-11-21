using CoreWPF.MVVM;
using CoreWPF.Utilites;
using MlsExclusive.Utilites;

namespace MlsExclusive.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private string login;
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
        public string User_agent
        {
            get { return this.user_agent; }
            set
            {
                this.user_agent = value;
                this.OnPropertyChanged("User_agent");
            }
        }

        public SettingsViewModel()
        {
            this.Title = "Настройки";
            User current_user = App.GetUser();

            this.login = current_user.login;
            this.password = current_user.password;
            this.user_agent = current_user.user_agent;
        }

        public override RelayCommand Command_save
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    App.SetUser(new User(this.Login, this.Password, this.User_agent));
                    base.Command_save?.Execute();
                });
            }
        }
    }
}

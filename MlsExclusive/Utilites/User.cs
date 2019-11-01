namespace MlsExclusive.Utilites
{
    public struct User
    {
        public string login { get; set; }
        public string password { get; set; }
        public string user_agent { get; set; }

        public User(string login, string password, string user_agent)
        {
            this.login = login;
            this.password = password;
            this.user_agent = user_agent;
        }
    }
}

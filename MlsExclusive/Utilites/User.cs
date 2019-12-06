namespace MlsExclusive.Utilites
{
    /// <summary>
    /// Объект с данными для авторизации на сервере МЛС
    /// </summary>
    public struct User
    {
        /// <summary>
        /// Логин для авторизации.
        /// </summary>
        public string login { get; set; }

        /// <summary>
        /// Пароль для авторизации.
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// User agent, используется в запросах к МЛС серверу.
        /// </summary>
        public string user_agent { get; set; }

        /// <summary>
        /// Создание нового пользователя.
        /// </summary>
        /// <param name="login">Логин для авторизации.</param>
        /// <param name="password">Пароль для авторизации.</param>
        /// <param name="user_agent">User agent, используется в запросах к МЛС серверу.</param>
        public User(string login, string password, string user_agent)
        {
            this.login = login;
            this.password = password;
            this.user_agent = user_agent;
        }
    }
}

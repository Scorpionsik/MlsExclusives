using MessagePack;

namespace MlsExclusive.Utilites
{
    /// <summary>
    /// Объект с данными для авторизации на сервере МЛС
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public struct User
    {
        /// <summary>
        /// Логин для авторизации.
        /// </summary>
        public string login { get; private set; }

        /// <summary>
        /// Пароль для авторизации.
        /// </summary>
        public string password { get; private set; }

        /// <summary>
        /// User agent, используется в запросах к МЛС серверу.
        /// </summary>
        public string user_agent { get; private set; }

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

        /// <summary>
        /// Преобразует <see cref="User"/> в json строку.
        /// </summary>
        /// <param name="user"><see cref="User"/> для сериализации.</param>
        /// <returns>Возвращает соответствующую json строку.</returns>
        public static string Serialize(User user)
        {
            byte[] bytes = MessagePackSerializer.Serialize(user);
            return MessagePackSerializer.ToJson(bytes);
        }

        /// <summary>
        /// Преобразует json строку в <see cref="User"/>.
        /// </summary>
        /// <param name="json">Json строка для десериализации.</param>
        /// <returns>Возвращает соответствующий <see cref="User"/>.</returns>
        public static User Deserialize(string json)
        {
            return MessagePackSerializer.Deserialize<User>(MessagePackSerializer.FromJson(json), MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Instance);
        }
    }
}

namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Режимы сохранения для агенств.
    /// </summary>
    public enum AgencySerializeMode
    {
        /// <summary>
        /// Формат *.agency, использует встроенные методы сериализации.
        /// </summary>
        Default,
        /// <summary>
        /// Формат *.json, использует <see cref="MessagePack.MessagePackSerializer"/> для сериализации и преобразования в формат json.
        /// </summary>
        MessagePack,
        /// <summary>
        /// Формат *.agnc, использует <see cref="MessagePack.MessagePackSerializer"/> для сериализации и сохранении в бинарный файл.
        /// </summary>
        MessagePackNotJson
    }
}

using System;

namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Значения флага для обозначения, из какого МЛС фида был получен объект.
    /// </summary>
    [Serializable]
    public enum MlsMode
    {
        /// <summary>
        /// Объект взят из фида <see cref="MlsServer.Flats"/>.
        /// </summary>
        Flat,

        /// <summary>
        /// Объект взят из фида <see cref="MlsServer.Houses"/>.
        /// </summary>
        House
    }
}

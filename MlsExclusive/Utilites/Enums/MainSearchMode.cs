using MlsExclusive.Models;

namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Значения флага для поиска в выбранных объектах.
    /// </summary>
    public enum MainSearchMode
    {
        /// <summary>
        /// Искать по введенному тексту.
        /// </summary>
        Text,

        /// <summary>
        /// Искать по введенной цене.
        /// </summary>
        Price,

        /// <summary>
        /// Искать по <see cref="MlsOffer.Id"/>.
        /// </summary>
        Id
    }
}

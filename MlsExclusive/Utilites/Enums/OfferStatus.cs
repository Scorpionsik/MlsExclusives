using System;

namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Значения флага, обозначающие статус объявления МЛС.
    /// </summary>
    [Serializable]
    public enum OfferStatus
    {
        /// <summary>
        /// После последней выгрузки из МЛС, текущее объявление является добавленным.
        /// </summary>
        New,

        /// <summary>
        /// После последней выгрузки из МЛС в текущем объявлении не было никаких изменений.
        /// </summary>
        NoChanges,

        /// <summary>
        /// После последней выгрузки из МЛС в текущем объявлении были изменения.
        /// </summary>
        Modify,

        /// <summary>
        /// После последней выгрузки из МЛС текущее объявление не было найдено в фидах <see cref="MlsServer"/>.
        /// </summary>
        Delete
    }
}

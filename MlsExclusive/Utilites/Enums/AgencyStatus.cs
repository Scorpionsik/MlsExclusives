using MessagePack;
using MlsExclusive.Models;
using System;

namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Значения флага для обозначения новизны добавленного агенства.
    /// </summary>
    [Serializable]
    public enum AgencyStatus
    {
        /// <summary>
        /// С последней выгрузки из МЛС, текущее агенство является добавленным.
        /// </summary>
        New,
        /// <summary>
        /// С последней выгрузки из МЛС, текущее агенство уже было добавлено в коллекцию агенств.
        /// </summary>
        Old,
        /// <summary>
        /// С последней выгрузки из МЛС, внутри объявлений агенства произошли изменения.
        /// </summary>
        Edit
    }
}

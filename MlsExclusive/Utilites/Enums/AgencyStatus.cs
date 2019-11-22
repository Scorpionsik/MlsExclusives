namespace MlsExclusive.Utilites.Enums
{
    /// <summary>
    /// Значения флага для обозначения новизны добавленного агенства.
    /// </summary>
    public enum AgencyStatus
    {
        /// <summary>
        /// С последней выгрузки из МЛС, текущее агенство является добавленным.
        /// </summary>
        New,

        /// <summary>
        /// С последней выгрузки из МЛС, текущее агенство уже было добавлено в коллекцию агенств.
        /// </summary>
        Old
    }
}

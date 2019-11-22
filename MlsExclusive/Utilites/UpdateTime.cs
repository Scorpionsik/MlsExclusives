using CoreWPF.Utilites;
using System;
using System.IO;

namespace MlsExclusive.Utilites
{
    /// <summary>
    /// Инструмент для обновления времени последней выгрузки из МЛС.
    /// </summary>
    public static class UpdateTime
    {
        private const string UnixTimestampPath = "Data/last_update.time";

        /// <summary>
        /// Возвращает время последней выгрузки из МЛС.
        /// </summary>
        /// <returns>Возвращает время последней выгрузки из МЛС в формате <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset Get()
        {
            return UnixTime.ToDateTimeOffset(Convert.ToDouble(File.ReadAllText(UnixTimestampPath)), App.Timezone);
        }

        /// <summary>
        /// Устанавливает время последней выгрузки из МЛС.
        /// </summary>
        /// <param name="timestamp">Unix Timestamp со смещением во времени по <see cref="UnixTime.UTC"/>.</param>
        public static void Set(double timestamp)
        {
            File.WriteAllText(UnixTimestampPath, timestamp.ToString());
        }
    }
}

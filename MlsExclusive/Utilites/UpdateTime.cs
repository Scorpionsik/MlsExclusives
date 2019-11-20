using CoreWPF.Utilites;
using System;
using System.IO;

namespace MlsExclusive.Utilites
{
    public static class UpdateTime
    {
        private const string UnixTimestampPath = "Data/last_update.time";

        public static DateTimeOffset Get()
        {
            return UnixTime.ToDateTimeOffset(Convert.ToDouble(File.ReadAllText(UnixTimestampPath)), App.Timezone);
        }

        public static void Set(double timestamp)
        {
            File.WriteAllText(UnixTimestampPath, timestamp.ToString());
        }
    }
}

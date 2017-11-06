using System;

namespace Huobi
{
    internal static class UnixTime
    {
		private static DateTime st_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static uint Now { get { return GetFromDateTime(DateTime.UtcNow); } }
        public static uint GetFromDateTime(DateTime d) { return (uint)(d - st_unixEpoch).TotalSeconds; }
        public static DateTime ConvertToDateTime(uint unixtime) { return st_unixEpoch.AddSeconds(unixtime); }
    }
}

using System;

namespace BtcE
{
    internal static class UnixTime
    {
		private static DateTime st_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static UInt32 Now { get { return GetFromDateTime(DateTime.UtcNow); } }
        public static UInt32 GetFromDateTime(DateTime d) { return (UInt32)(d - st_unixEpoch).TotalSeconds; }
        public static DateTime ConvertToDateTime(UInt32 unixtime) { return st_unixEpoch.AddSeconds(unixtime); }
    }
}

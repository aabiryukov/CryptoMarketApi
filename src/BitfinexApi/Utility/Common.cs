using System;

namespace Bitfinex.Utility
{
	public static class Common
	{
		public static long UnixTimeStampUtc()
		{
			var currentTime = DateTime.Now;
			var dt = currentTime.ToUniversalTime();
			var unixEpoch = new DateTime(1970, 1, 1);
			var unixTimeStamp = (Int32)(dt.Subtract(unixEpoch)).TotalSeconds;
			return unixTimeStamp;
		}

		public static double GetTimeStamp(DateTime dt)
		{
			var unixEpoch = new DateTime(1970, 1, 1);
			return dt.Subtract(unixEpoch).TotalSeconds;
		}

	}
}

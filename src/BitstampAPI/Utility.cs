using System.Globalization;

namespace Bitstamp
{
	internal static class Utility
	{
		public static decimal StringToDecimal(string value)
		{
			return decimal.Parse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
		}
	}
}

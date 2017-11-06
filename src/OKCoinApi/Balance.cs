using System.Globalization;
using Newtonsoft.Json;

namespace OKCoin
{
	public class Balance
	{
//		[JsonProperty("btc", Required = Required.Always)]
		public decimal Btc { get; set; }
//		[JsonProperty("usd", Required = Required.Always)]
		public decimal Usd { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}$ {1}btc", Usd, Btc);
		}
	}
}

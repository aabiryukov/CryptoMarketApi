using Newtonsoft.Json;

namespace Bitstamp
{
	public class Balance
	{
		[JsonProperty("usd_balance", Required = Required.AllowNull)]
		public decimal UsdBalance { get; set; }

		[JsonProperty("usd_reserved", Required = Required.AllowNull)]
		public decimal UsdReserved { get; set; }

		[JsonProperty("btc_reserved", Required = Required.AllowNull)]
		public decimal BtcReserved { get; set; }

		[JsonProperty("usd_available", Required = Required.AllowNull)]
		public decimal UsdAvailable { get; set; }

		[JsonProperty("btc_available", Required = Required.AllowNull)]
		public decimal BtcAvailable { get; set; }

		[JsonProperty("fee", Required = Required.AllowNull)]
		public decimal Fee { get; set; }
	}
}

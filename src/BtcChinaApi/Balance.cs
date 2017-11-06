using Newtonsoft.Json;

namespace BTCChina
{
	public class Balance
	{
		[JsonProperty("btc", Required = Required.Always)]
		public CurrencyValue Btc { get; private set; }
		[JsonProperty("cny", Required = Required.Always)]
		public CurrencyValue Cny { get; private set; }
	}
}

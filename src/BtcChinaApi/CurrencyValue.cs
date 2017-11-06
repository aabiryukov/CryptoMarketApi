using Newtonsoft.Json;

namespace BTCChina
{
	public class CurrencyValue
	{
		[JsonProperty("currency", Required = Required.Always)]
		public string Name { get; private set; }
		[JsonProperty("symbol", Required = Required.Always)]
		public string Symbol { get; private set; }
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }

		public override string ToString()
		{
			return Amount + Name;
		}
	}
}

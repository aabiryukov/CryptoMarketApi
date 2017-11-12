using System;
using System.Globalization;
using Newtonsoft.Json;

namespace BTCChina
{
	public class Trade
	{
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("date", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("tid", Required = Required.Always)]
		public UInt32 Tid { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}: {1} / {2}btc", Date, Price, Amount);
		}
	}

	public class TradeHistory
	{
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("date", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("tid", Required = Required.Always)]
		public UInt32 Tid { get; private set; }
		[JsonProperty("type", Required = Required.Always)]
		public string TradeType { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}: {1} / {2}btc", Date, Price, Amount);
		}
	}
}

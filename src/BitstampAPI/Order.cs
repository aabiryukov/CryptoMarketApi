using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitstamp
{
	public class Order
	{
		[JsonProperty("id")]
		public int OrderId { get; set; }

		// [JsonProperty("datetime", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		[JsonProperty("datetime", Required = Required.Always)]
		public string Date { get; private set; }
		[JsonProperty("type", Required = Required.Always)]
//		[JsonConverter(typeof(StringEnumConverter))]
		public int OrderType { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }

		public override string ToString()
		{
			return (new { Date, OrderId, OrderType, Price, Amount }).ToString();
		}

	}

	public enum TradeType
	{
		Buy = 0,
		Sell = 1
	}
}

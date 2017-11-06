using Newtonsoft.Json;
using System;

namespace OKCoin
{
	public class Order
	{
		[JsonProperty("order_id", Required = Required.Always)]
		public int OrderId { get; private set; }
		[JsonProperty("type", Required = Required.Always)]
		public string OrderType { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("symbol", Required = Required.Always)]
		public string Symbol { get; private set; }
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }

		[JsonProperty("create_date", Required = Required.Always)]
		[JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }
		[JsonProperty("status", Required = Required.Always)]
		public int Status { get; private set; }
/*
		public static Order ReadFromJObject(JToken o)
		{
			if (o == null)
				return null;
			return new Order()
			{
				OrderId = o.Value<int>("id"),
				OrderType = o.Value<string>("type"),
				Price = o.Value<decimal>("price"),
				Currency = o.Value<string>("currency"),
				Amount = o.Value<decimal>("amount"),
				OriginalAmount = o.Value<decimal>("amount_original"),
				Date = o.Value<UInt32>("date"),
				Status = o.Value<string>("status"),
			};
		}
 */ 
	}
}

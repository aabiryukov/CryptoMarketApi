using Newtonsoft.Json.Linq;
using System;

namespace BTCChina
{
	public class Order
	{
		public int OrderId { get; private set; }
		public string OrderType { get; private set; }
		public decimal Price { get; private set; }
		public string Currency { get; private set; }
		public decimal Amount { get; private set; }
		public decimal OriginalAmount { get; private set; }
		public UInt32 Date { get; private set; }
		public string Status { get; private set; }

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
	}
}

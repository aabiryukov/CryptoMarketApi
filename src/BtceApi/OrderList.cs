using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace BtcE
{
	public class Order
	{
		[JsonProperty("pair", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public BtcePair Pair { get; private set; }
		[JsonProperty("type", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public TradeType TradeType { get; private set; }
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("rate", Required = Required.Always)]
		public decimal Rate { get; private set; }
		[JsonProperty("timestamp_created", Required = Required.Always)]
		public UInt32 TimestampCreated { get; private set; }
		[JsonProperty("status", Required = Required.Always)]
		public int Status { get; private set; }
/*
		public static Order ReadFromJObject(JObject o) {
			if ( o == null )
				return null;
			return new Order() {
				Pair = BtcePairHelper.FromString(o.Value<string>("pair")),
				Type = TradeTypeHelper.FromString(o.Value<string>("type")),
				Amount = o.Value<decimal>("amount"),
				Rate = o.Value<decimal>("rate"),
				TimestampCreated = o.Value<UInt32>("timestamp_created"),
				Status = o.Value<int>("status")
			};
		}
 */ 
	}

	public class OrderList : Dictionary<int, Order>
	{
/*
		public Dictionary<int, Order> List { get; private set; }
		public static OrderList ReadFromJObject(JObject o)
		{
			var orders = new Dictionary<int, Order>();
			foreach (var item in o)
			{
				var orderId = int.Parse(item.Key, CultureInfo.InvariantCulture);
				var order = Order.ReadFromJObject(item.Value as JObject);
				orders.Add(orderId, order);
			}

			return new OrderList
			{
				List = orders
			};
		}
*/
	}
}

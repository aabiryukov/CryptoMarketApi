using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BtcE
{
	public class Trade
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
		[JsonProperty("order_id", Required = Required.Always)]
		public int OrderId { get; private set; }
		[JsonProperty("is_your_order", Required = Required.Always)]
		public bool IsYourOrder { get; private set; }
		[JsonProperty("timestamp", Required = Required.Always)]
		public UInt32 Timestamp { get; private set; }
/*
		public static Trade ReadFromJObject(JObject o) {
			if ( o == null )
				return null;
			return new Trade() {
				Pair = BtcePairHelper.FromString(o.Value<string>("pair")),
				Type = TradeTypeHelper.FromString(o.Value<string>("type")),
				Amount = o.Value<decimal>("amount"),
				Rate = o.Value<decimal>("rate"),
				Timestamp = o.Value<UInt32>("timestamp"),
				IsYourOrder = o.Value<int>("is_your_order") == 1,
				OrderId = o.Value<int>("order_id")
			};
		}
 */ 
	}
	public class TradeHistory: Dictionary<int, Trade>
	{
/*
		public Dictionary<int, Trade> List { get; private set; }
		public static TradeHistory ReadFromJObject(JObject o) {
			return new TradeHistory() {
				List = o.OfType<KeyValuePair<string, JToken>>().ToDictionary(item => int.Parse(item.Key), item => Trade.ReadFromJObject(item.Value as JObject))
			};
		}
 */ 
	}
}

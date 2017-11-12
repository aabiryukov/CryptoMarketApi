using Newtonsoft.Json;
using System;

namespace Huobi
{
	public class Order
	{
        public enum HuobiOrderType
        {
            Buy = 1,
            Sell = 2
        }

        [JsonProperty("id", Required = Required.Always)]
		public int OrderId { get; private set; }
		[JsonProperty("type", Required = Required.Always)]
		public int InternalType { get; private set; }
		[JsonProperty("order_price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("order_amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
        [JsonProperty("processed_amount", Required = Required.Always)]
        public decimal ProcessedAmount { get; private set; }
        [JsonProperty("order_time", Required = Required.Always)]
		[JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }

	    public HuobiOrderType OrderType => (HuobiOrderType) InternalType;

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

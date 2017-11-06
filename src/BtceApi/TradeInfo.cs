using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
namespace BtcE
{
	// ReSharper disable UnusedAutoPropertyAccessor.Local
	public class TradeInfo
	{
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("date", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }
		[JsonProperty("item", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public BtceCurrency Item { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("price_currency", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public BtceCurrency PriceCurrency { get; private set; }
		[JsonProperty("tid", Required = Required.Always)]
		public UInt32 Tid { get; private set; }
		[JsonProperty("trade_type", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public TradeInfoType TradeType { get; private set; }
	}
	// ReSharper restore UnusedAutoPropertyAccessor.Local

    public class TradeInfoV3
    {
        public decimal Amount { get; private set; }
        public DateTime Timestamp { get; private set; }
        public decimal Price { get; private set; }
        public UInt32 Tid { get; private set; }
        public TradeInfoType TradeType { get; private set; }

        public static TradeInfoV3 ReadFromJObject(JObject o)
        {
            if (o == null)
                return null;

            return new TradeInfoV3
            {
                Amount = o.Value<decimal>("amount"),
                Price = o.Value<decimal>("price"),
                Timestamp = UnixTime.ConvertToDateTime(o.Value<UInt32>("timestamp")),
                Tid = o.Value<UInt32>("tid"),
                TradeType = TradeInfoTypeHelper.FromString(o.Value<string>("type"))
            };
        }

	    public override string ToString()
	    {
		    return string.Format(CultureInfo.InvariantCulture, "{0}: {1} / {2}btc", Timestamp, Price, Amount);
	    }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace BtcE
{
    public class Transaction
    {
		[JsonProperty("type", Required = Required.Always)]
        public int TradeType { get; private set; }
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("currency", Required = Required.Always)]
		[JsonConverter(typeof(StringEnumConverter))]
		public BtceCurrency Currency { get; private set; }
		[JsonProperty("desc", Required = Required.Default)]
		public string Description { get; private set; }
		[JsonProperty("status", Required = Required.Always)]
		public int Status { get; private set; }
		[JsonProperty("timestamp", Required = Required.Always)]
		public UInt32 Timestamp { get; private set; }
/*
        public static Transaction ReadFromJObject(JObject o)
        {
            if (o == null)
                return null;
			return new Transaction()
            {
                Type = o.Value<int>("type"),
                Amount = o.Value<decimal>("amount"),
                Currency = BtceCurrencyHelper.FromString(o.Value<string>("currency")),
                Timestamp = o.Value<UInt32>("timestamp"),
                Status = o.Value<int>("status"),
                Description = o.Value<string>("desc")
            };
        }
 */ 
    }

	public class TransHistory : Dictionary<int, Transaction>
	{
/*
        public Dictionary<int, Transaction> List { get; private set; }
        public static TransHistory ReadFromJObject(JObject o)
        {
			var result = new TransHistory() {
				List = o.OfType<KeyValuePair<string,JToken>>().ToDictionary(a=>int.Parse(a.Key), a=>Transaction.ReadFromJObject(a.Value as JObject))
			};

			var x = JsonConvert.DeserializeObject<Dictionary<int, Transaction>>(o.ToString());

	        return result;
        }
 */ 
    }
}

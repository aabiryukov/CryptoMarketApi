using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace BtcE
{
	[JsonConverter(typeof(OrderInfoJsonConverter))]
	public class OrderInfo
	{
		public decimal Price { get; private set; }
		public decimal Amount { get; private set; }
		public static OrderInfo ReadFromJObject(JArray o) {
			if ( o == null )
				return null;
			return new OrderInfo() {
				Price = o.Value<decimal>(0),
				Amount = o.Value<decimal>(1),
			};
		}

        public override string ToString()
        {
            return $"{Amount}btc / {Price}";
        }

        private class OrderInfoJsonConverter : JsonConverter
		{
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader == null) throw new ArgumentNullException("reader");
				if (serializer == null) throw new ArgumentNullException("serializer");

				if (reader.TokenType == JsonToken.StartArray)
				{
					var orderInfo = new OrderInfo();
					var instance = (decimal[])serializer.Deserialize(reader, typeof(decimal[]));
					orderInfo.Price = instance[0];
					orderInfo.Amount = instance[1];
					return orderInfo;
				}

				throw new BtcApiException("OrderInfoJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
			}

			public override bool CanConvert(Type objectType)
			{
				throw new NotImplementedException();
			}
		}
	}
	public class Depth
	{
		[JsonProperty("asks")]
		public List<OrderInfo> Asks { get; private set; }
		[JsonProperty("bids")]
		public List<OrderInfo> Bids { get; private set; }
		public static Depth ReadFromJObject(JObject jDepth) {
            if (jDepth == null) throw new ArgumentNullException(nameof(jDepth));
            return new Depth() {
				Asks = jDepth["asks"].OfType<JArray>().Select(order => OrderInfo.ReadFromJObject(order)).ToList(),
				Bids = jDepth["bids"].OfType<JArray>().Select(order => OrderInfo.ReadFromJObject(order)).ToList()
			};
		}
	}
}

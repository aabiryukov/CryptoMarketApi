using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Bitstamp
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
					var instance = (string[])serializer.Deserialize(reader, typeof(string[]));
					orderInfo.Price = Utility.StringToDecimal(instance[0]);
					orderInfo.Amount = Utility.StringToDecimal(instance[1]);
					return orderInfo;
				}

				throw new BitstampException("OrderInfoJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
			}

			public override bool CanConvert(Type objectType)
			{
				throw new NotImplementedException();
			}
		}
	}

	public class OrderBook
	{
		[JsonProperty("timestamp", Required = Required.Always)]
		public UInt32 Timestamp { get; private set; }

		[JsonProperty("asks")]
		public List<OrderInfo> Asks { get; private set; }

		[JsonProperty("bids")]
		public List<OrderInfo> Bids { get; private set; }
	}
}

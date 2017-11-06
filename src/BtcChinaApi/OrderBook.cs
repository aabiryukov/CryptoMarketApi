using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BTCChina
{
	[JsonConverter(typeof(OrderInfoJsonConverter))]
	public class OrderInfo
	{
		public decimal Price { get; private set; }

		public decimal Amount { get; private set; }

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

				throw new BTCChinaException("OrderInfoJsonConverter.ReadJson", "Unknown", "OrderInfoJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
			}

			public override bool CanConvert(Type objectType)
			{
				throw new NotImplementedException();
			}
		}
	}

	public class OrderBook
	{
		[JsonProperty("date", Required = Required.Always)]
		public UInt32 Timestamp { get; private set; }

		[JsonProperty("asks")]
		public List<OrderInfo> Asks { get; private set; }

		[JsonProperty("bids")]
		public List<OrderInfo> Bids { get; private set; }
	}
}

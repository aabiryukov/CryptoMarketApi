using System;
using Newtonsoft.Json;

namespace BTCChina
{
	class UnixTimeJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (serializer == null) throw new ArgumentNullException("serializer");

//			if (reader.TokenType == JsonToken.StartObject)
			{
				var instance = (UInt32)serializer.Deserialize(reader, typeof(UInt32));
				return UnixTime.ConvertToDateTime(instance);
			}

//			throw new BitstampException("UnixTimeJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}

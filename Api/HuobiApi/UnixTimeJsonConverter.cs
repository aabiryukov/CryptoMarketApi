using System;
using Newtonsoft.Json;

namespace Huobi
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
				var instance = (UInt64)serializer.Deserialize(reader, typeof(UInt64));
				return UnixTime.ConvertToDateTime((UInt32)(instance / 1000));
			}

//			throw new BitstampException("UnixTimeJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}

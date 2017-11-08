using System;
using Newtonsoft.Json;

namespace Wex
{
	class UnixTimeJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			if (serializer == null) throw new ArgumentNullException("serializer");

			if (reader.TokenType == JsonToken.Integer)
			{
				var instance = (UInt32)serializer.Deserialize(reader, typeof(UInt32));
				return UnixTime.ConvertToDateTime(instance);
			}

			throw new WexApiException("UnixTimeJsonConverter: Unexpected reader.TokenType=" + reader.TokenType);
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wex
{
	public class Rights
	{
		[JsonProperty("info", Required = Required.Always)]
		public bool Info { get; private set; }
		[JsonProperty("trade", Required = Required.Always)]
		public bool Trade { get; private set; }
/*
		public static Rights ReadFromJObject(JObject o) {
			if ( o == null )
				return null;
			return new Rights() {
				Info = o.Value<int>("info") == 1,
				Trade = o.Value<int>("trade") == 1
			};
		}
 */ 
	}
}

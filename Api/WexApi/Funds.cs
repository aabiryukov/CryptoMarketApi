using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wex
{
	public class Funds
	{
		[JsonProperty("btc", Required = Required.Always)]
        public decimal Btc { get; private set; }
		[JsonProperty("ltc")]
		public decimal Ltc { get; private set; }
		[JsonProperty("nmc")]
		public decimal Nmc { get; private set; }
		[JsonProperty("nvc")]
		public decimal Nvc { get; private set; }
        [JsonProperty("dsh")]
        public decimal Dsh { get; private set; }
        [JsonProperty("zec")]
        public decimal Zec { get; private set; }
        [JsonProperty("bch", Required = Required.Always)]
        public decimal Bch { get; private set; }
        [JsonProperty("usd", Required = Required.Always)]
        public decimal Usd { get; private set; }
		[JsonProperty("rur")]
		public decimal Rur { get; private set; }
		[JsonProperty("eur")]
		public decimal Eur { get; private set; }

		public static Funds ReadFromJObject(JObject o) {
			if ( o == null )
				return null;
			return new Funds() {
                Btc = o.Value<decimal>("btc"),
                Ltc = o.Value<decimal>("ltc"),
                Nmc = o.Value<decimal>("ntc"),
                Nvc = o.Value<decimal>("nvc"),
                Dsh = o.Value<decimal>("dsh"),
                Zec = o.Value<decimal>("zec"),
                Bch = o.Value<decimal>("bch"),
                Usd = o.Value<decimal>("usd"),
                Rur = o.Value<decimal>("rur"),
                Eur = o.Value<decimal>("eur")
			};
		} 
	};

}

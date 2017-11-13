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

        // Wex tokens
        [JsonProperty("btcet")]
        public decimal Btcet { get; private set; }
        [JsonProperty("ltcet")]
        public decimal Ltcet { get; private set; }
        [JsonProperty("nmcet")]
        public decimal Nmcet { get; private set; }
        [JsonProperty("nvcet")]
        public decimal Nvcet { get; private set; }
        [JsonProperty("ppcet")]
        public decimal Ppcet { get; private set; }
        [JsonProperty("dshet")]
        public decimal Dshet { get; private set; }
        [JsonProperty("ethet")]
        public decimal Ethet { get; private set; }
        [JsonProperty("bchet")]
        public decimal Bchet { get; private set; }
        [JsonProperty("usdet")]
        public decimal Usdet { get; private set; }
        [JsonProperty("ruret")]
        public decimal Ruret { get; private set; }
        [JsonProperty("euret")]
        public decimal Euret { get; private set; }

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
                Eur = o.Value<decimal>("eur"),

                // Tokens
                Btcet = o.Value<decimal>("btcet"),
                Ltcet = o.Value<decimal>("ltcet"),
                Nmcet = o.Value<decimal>("nmcet"),
                Nvcet = o.Value<decimal>("nvcet"),
                Ppcet = o.Value<decimal>("ppcet"),
                Dshet = o.Value<decimal>("dshet"),
                Ethet = o.Value<decimal>("ethet"),
                Bchet = o.Value<decimal>("bchet"),
                Usdet = o.Value<decimal>("usdet"),
                Ruret = o.Value<decimal>("ruret"),
                Euret = o.Value<decimal>("euret"),
            };
		} 
	};

}

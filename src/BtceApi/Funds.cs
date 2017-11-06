using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BtcE
{
	public class Funds
	{
		[JsonProperty("btc", Required = Required.Always)]
        public decimal Btc { get; private set; }
		[JsonProperty("ltc")]
		public decimal Ltc { get; private set; }
		[JsonProperty("ntc")]
		public decimal Nmc { get; private set; }
		[JsonProperty("nvc")]
		public decimal Nvc { get; private set; }
		[JsonProperty("trc")]
		public decimal Trc { get; private set; }
		[JsonProperty("ppc")]
		public decimal Ppc { get; private set; }
		[JsonProperty("ftc")]
		public decimal Ftc { get; private set; }
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
                Trc = o.Value<decimal>("trc"),
                Ppc = o.Value<decimal>("ppc"),
                Ftc = o.Value<decimal>("ftc"),
                Usd = o.Value<decimal>("usd"),
                Rur = o.Value<decimal>("rur"),
                Eur = o.Value<decimal>("eur")
			};
		} 
	};

}

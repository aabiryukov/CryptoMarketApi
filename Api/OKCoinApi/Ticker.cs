using Newtonsoft.Json.Linq;
using System;

namespace OKCoin
{
	public class Ticker
	{
		public decimal Buy { get; private set; }
		public decimal High { get; private set; }
		public decimal Last { get; private set; }
		public decimal Low { get; private set; }
		public decimal Sell { get; private set; }
		public decimal Volume { get; private set; }
		
		public DateTime ServerTime { get; private set; }
		public static Ticker ReadFromJObject(JToken jobj) {
			if ( jobj == null )
				throw new ArgumentNullException("jobj");

			var o = jobj["ticker"];

			if (o == null)
				return null;

			var ticker = new Ticker
			{
				Buy = o.Value<decimal>("buy"),
				High = o.Value<decimal>("high"),
				Last = o.Value<decimal>("last"),
				Low = o.Value<decimal>("low"),
				Sell = o.Value<decimal>("sell"),
				Volume = o.Value<decimal>("vol"),
				ServerTime = UnixTime.ConvertToDateTime(jobj["date"].Value<uint>()),
			};

			return ticker;
		}
	}
}

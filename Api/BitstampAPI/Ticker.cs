using Newtonsoft.Json.Linq;
using System;

namespace Bitstamp
{
	public class Ticker
	{
		public decimal Average { get; private set; }
		public decimal Buy { get; private set; }
		public decimal High { get; private set; }
		public decimal Last { get; private set; }
		public decimal Low { get; private set; }
		public decimal Sell { get; private set; }
		public decimal Volume { get; private set; }
//		public decimal VolumeCurrent { get; private set; }
		public UInt32 ServerTime { get; private set; }
		public static Ticker ReadFromJObject(JObject o) {
			if ( o == null )
				return null;
			return new Ticker() {
				Average = o.Value<decimal>("vwap"),
				Buy = o.Value<decimal>("ask"),
				High = o.Value<decimal>("high"),
				Last = o.Value<decimal>("last"),
				Low = o.Value<decimal>("low"),
				Sell = o.Value<decimal>("bid"),
				Volume = o.Value<decimal>("volume"),
				ServerTime = o.Value<UInt32>("timestamp"),
			};
		}
	}
}

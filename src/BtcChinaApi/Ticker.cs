using Newtonsoft.Json.Linq;
using System;

namespace BTCChina
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
//		public decimal PreviousClose { get; private set; }
		public decimal Open { get; private set; }
		
		public UInt32 ServerTime { get; private set; }
		public static Ticker ReadFromJObject(JToken o) {
			if ( o == null )
				return null;
			return new Ticker() {
				Average = o.Value<decimal>("vwap"),
				Buy = o.Value<decimal>("buy"),
				High = o.Value<decimal>("high"),
				Last = o.Value<decimal>("last"),
				Low = o.Value<decimal>("low"),
				Sell = o.Value<decimal>("sell"),
				Volume = o.Value<decimal>("vol"),
				Open = o.Value<decimal>("open"),
//				PreviousClose = o.Value<decimal>("prev_close"),
				ServerTime = o.Value<UInt32>("date"),
			};
		}
	}
}

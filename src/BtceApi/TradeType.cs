using System;

namespace BtcE
{
	public enum TradeType
	{
		Sell,
		Buy
	}
	internal static class TradeTypeHelper
	{
/*
		public static TradeType FromString(string tradeType) {
			switch (tradeType) {
				case "sell":
					return TradeType.Sell;
				case "buy":
					return TradeType.Buy;
				default:
					throw new ArgumentException("invalid value", "tradeType");
			}
		}
*/
		public static string ToString(TradeType tradeType) {
			return Enum.GetName(typeof(TradeType), tradeType).ToLowerInvariant();
		}
	}
}

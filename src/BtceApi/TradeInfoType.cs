using System;
namespace BtcE
{
    public enum TradeInfoType
    {
        Ask,
        Bid
    }

    internal static class TradeInfoTypeHelper
    {
        public static TradeInfoType FromString(string tradeType)
        {
            switch (tradeType)
            {
                case "ask":
                    return TradeInfoType.Ask;
                case "bid":
                    return TradeInfoType.Bid;
                default:
                    throw new ArgumentException("Unknown argument tradeType");
            }
        }
    }
}

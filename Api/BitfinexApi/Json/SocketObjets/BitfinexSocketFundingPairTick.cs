using Bitfinex.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Json.SocketObjets
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSocketFundingPairTick
    {
        [BitfinexProperty(0)]
        public decimal FlashReturnRate { get; set; }
        [BitfinexProperty(1)]
        public decimal Bid { get; set; }
        [BitfinexProperty(2)]
        public int BidPeriod { get; set; }
        [BitfinexProperty(3)]
        public decimal BidSize { get; set; }
        [BitfinexProperty(4)]
        public decimal Ask { get; set; }
        [BitfinexProperty(5)]
        public int AskPeriod { get; set; }
        [BitfinexProperty(6)]
        public decimal AskSize { get; set; }
        [BitfinexProperty(7)]
        public decimal DailyChange { get; set; }
        [BitfinexProperty(8)]
        public decimal DailyChangePercentage { get; set; }
        [BitfinexProperty(9)]
        public decimal LastPrice { get; set; }
        [BitfinexProperty(10)]
        public decimal Volume { get; set; }
        [BitfinexProperty(11)]
        public decimal High { get; set; }
        [BitfinexProperty(12)]
        public decimal Low { get; set; }
    }
}

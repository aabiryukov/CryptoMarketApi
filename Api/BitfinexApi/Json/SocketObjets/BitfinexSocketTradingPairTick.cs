using Bitfinex.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Json.SocketObjets
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSocketTradingPairTick
    {
        [BitfinexProperty(0)]
        public decimal Bid { get; set; }
        [BitfinexProperty(1)]
        public decimal BidSize { get; set; }
        [BitfinexProperty(2)]
        public decimal Ask { get; set; }
        [BitfinexProperty(3)]
        public decimal AskSize { get; set; }
        [BitfinexProperty(4)]
        public decimal DailyChange { get; set; }
        [BitfinexProperty(5)]
        public decimal DailyChangePercentage { get; set; }
        [BitfinexProperty(6)]
        public decimal LastPrice { get; set; }
        [BitfinexProperty(7)]
        public decimal Volume { get; set; }
        [BitfinexProperty(8)]
        public decimal High { get; set; }
        [BitfinexProperty(9)]
        public decimal Low { get; set; }
    }
}

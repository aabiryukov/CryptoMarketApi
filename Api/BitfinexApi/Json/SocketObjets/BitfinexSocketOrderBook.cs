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
    public class BitfinexSocketOrderBook
    {
        [BitfinexProperty(0)]
        public double Price { get; set; }
        [BitfinexProperty(1)]
        public int Count { get; set; }
        [BitfinexProperty(2)]
        public double Amount { get; set; }
    }
}

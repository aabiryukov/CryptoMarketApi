﻿using System;
using Bitfinex.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Json.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexTradeSimple
    {
        [BitfinexProperty(0)]
        public long Id { get; set; }
        [BitfinexProperty(1), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [BitfinexProperty(2)]
        public decimal Amount { get; set; }
        [BitfinexProperty(3)]
        public decimal Price { get; set; }
    }
}

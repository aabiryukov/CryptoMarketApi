﻿using Bitfinex.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Json.SocketObjets
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexSocketEvent
    {
        [BitfinexProperty(0)]
        public long ChannelId { get; set; }
        [BitfinexProperty(1)]
        public string Event { get; set; }
        [BitfinexProperty(2)]
        public JToken Data { get; set; }
    }
}

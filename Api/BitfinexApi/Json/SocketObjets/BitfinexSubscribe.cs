using Newtonsoft.Json;

namespace Bitfinex.Json.SocketObjets
{
    public class BitfinexSubscribeRequest
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }

        public BitfinexSubscribeRequest(string channel)
        {
            Event = "subscribe";
            Channel = channel;
        }
    }

    public class BitfinexBookSubscribeRequest : BitfinexSubscribeRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("prec")]
        public string Precision { get; set; }

        [JsonProperty("freq")]
        public string Frequency { get; set; }

        [JsonProperty("len")]
        public int Length { get; set; }

        public BitfinexBookSubscribeRequest(string symbol, string precision, string frequency, int length) : base("book")
        {
            Symbol = symbol;
            Precision = precision;
            Frequency = frequency;
            Length = length;
        }
    }

    public class BitfinexTickerSubscribeRequest: BitfinexSubscribeRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexTickerSubscribeRequest(string symbol): base("ticker")
        {
            Symbol = symbol;
        }
    }

    public class BitfinexTradeSubscribeRequest : BitfinexSubscribeRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        public BitfinexTradeSubscribeRequest(string symbol) : base("trades")
        {
            Symbol = symbol;
        }
    }
}

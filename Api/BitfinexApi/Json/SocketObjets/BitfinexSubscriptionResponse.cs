using Newtonsoft.Json;

namespace Bitfinex.Json.SocketObjets
{
    public class BitfinexSubscriptionResponse
    {
        public string Event { get; set; }
        [JsonProperty("channel")]
        public string ChannelName { get; set; }
        [JsonProperty("chanId")]
        public long ChannelId { get; set; }
    }
}

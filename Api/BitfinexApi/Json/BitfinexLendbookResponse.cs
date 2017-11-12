
using Bitfinex.Json.LendbookTypes;
using Newtonsoft.Json;

namespace Bitfinex.Json
{
   public class BitfinexLendbookResponse
   {
      [JsonProperty("bids")]
      public Bid[] Bids { get; set; }

      [JsonProperty("asks")]
      public Ask[] Asks { get; set; }
   }
}

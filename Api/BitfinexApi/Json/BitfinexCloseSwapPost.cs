using Newtonsoft.Json;

namespace Bitfinex.Json
{
   public class BitfinexCloseSwapPost : BitfinexPostBase
   {
      [JsonProperty("swap_id")]
      public int SwapId { get; set; }
   }
}

using Bitfinex.Json.MultipleOrderTypes;
using Newtonsoft.Json;

namespace Bitfinex.Json
{
   public class BitfinexMultipleNewOrderResponse
   {
      [JsonProperty("order_ids")]
      public OrderId[] OrderIds { get; set; }

      [JsonProperty("status")]
      public string Status { get; set; }

   }
}

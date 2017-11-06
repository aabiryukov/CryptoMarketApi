using Newtonsoft.Json;

namespace Bitfinex.Json
{
   public class BitfinexOfferStatusPost : BitfinexPostBase
   {
      [JsonProperty("offer_id")]
      public int OfferId { get; set; }

   }
}

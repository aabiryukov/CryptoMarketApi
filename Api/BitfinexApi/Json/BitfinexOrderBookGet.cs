﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using Bitfinex.Json.OrderbookTypes;
using Newtonsoft.Json;

namespace Bitfinex.Json
{

   public class BitfinexOrderBookGet
   {

      [JsonProperty("bids")]
      public Bid[] Bids { get; set; }

      [JsonProperty("asks")]
      public Ask[] Asks { get; set; }
   }

}

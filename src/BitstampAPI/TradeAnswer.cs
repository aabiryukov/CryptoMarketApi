using System;
using Newtonsoft.Json;

namespace Bitstamp {
	public class TradeAnswer {
		[JsonProperty("id", Required = Required.Always)]
		public int OrderId { get; private set; }

//		[JsonProperty("datetime", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		[JsonProperty("datetime", Required = Required.Always)]
		public string OrderDate { get; set; }

		[JsonProperty("type", Required = Required.Always)]
		public bool TradeType { get; private set; }

		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }

		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
	}
}

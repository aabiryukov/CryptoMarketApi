using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitstamp
{
	public class Transaction
	{
		[JsonProperty("amount", Required = Required.Always)]
		public decimal Amount { get; private set; }
		[JsonProperty("date", Required = Required.Always), JsonConverter(typeof(UnixTimeJsonConverter))]
		public DateTime Date { get; private set; }
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; private set; }
		[JsonProperty("tid", Required = Required.Always)]
		public UInt32 Tid { get; private set; }
	}
}

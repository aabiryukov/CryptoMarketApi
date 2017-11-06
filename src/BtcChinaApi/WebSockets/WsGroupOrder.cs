using System.Collections.Generic;
using Newtonsoft.Json;

namespace BTCChina.WebSockets
{
	public class WsGroupOrder
	{
		[JsonProperty("market", Required = Required.Always)]
		public string Market { get; set; }
		[JsonProperty("ask", Required = Required.Always)]
		public List<WsOrder> Asks { get; set; }
		[JsonProperty("bid", Required = Required.Always)]
		public List<WsOrder> Bids { get; set; }
	}
}

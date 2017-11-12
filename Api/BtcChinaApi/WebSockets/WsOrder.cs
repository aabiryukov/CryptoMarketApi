using Newtonsoft.Json;

namespace BTCChina.WebSockets
{
	public class WsOrder
	{
		[JsonProperty("price", Required = Required.Always)]
		public decimal Price { get; set; }
		[JsonProperty("type", Required = Required.Always)]
		public string OrderType { get; set; }
		[JsonProperty("totalamount", Required = Required.Always)]
		public decimal TotalAmount { get; set; }
	}
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BtcE
{
    public class UserInfo
    {
		[JsonProperty("funds", Required = Required.Always)]
		public Funds Funds { get; private set; }

		[JsonProperty("rights", Required = Required.Always)]
		public Rights Rights { get; private set; }

		[JsonProperty("transaction_count", Required = Required.Always)]
		public int TransactionCount { get; private set; }

		[JsonProperty("open_orders", Required = Required.Always)]
		public int OpenOrders { get; private set; }
		
		[JsonProperty("server_time", Required = Required.Always)]
		public int ServerTime { get; private set; }
    }
}

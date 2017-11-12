using Newtonsoft.Json;

namespace BTCChina
{
	public class AccountInfo
	{
		[JsonProperty("profile", Required = Required.Always)]
		public AccountProfile Profile { get; private set; }
		[JsonProperty("balance", Required = Required.Always)]
		public Balance Balance { get; private set; }
		[JsonProperty("frozen", Required = Required.Always)]
		public Balance Frozen { get; private set; }
		[JsonProperty("loan", Required = Required.Always)]
		public Balance Loan { get; private set; }

	}
}

using Newtonsoft.Json;

namespace Huobi
{
	public class AccountInfo
	{
        [JsonProperty("total", Required = Required.Always)]
        public decimal Total { get; private set; }
        [JsonProperty("net_asset", Required = Required.Always)]
        public decimal NetAsset { get; private set; }
        [JsonProperty("available_cny_display", Required = Required.Always)]
        public decimal AvailableCny { get; private set; }
        [JsonProperty("available_btc_display", Required = Required.Always)]
        public decimal AvailableBtc { get; private set; }
        [JsonProperty("frozen_cny_display", Required = Required.Always)]
        public decimal FrozenCny { get; private set; }
        [JsonProperty("frozen_btc_display", Required = Required.Always)]
        public decimal FrozenBtc { get; private set; }
        [JsonProperty("loan_cny_display", Required = Required.Always)]
        public decimal LoanCny { get; private set; }
        [JsonProperty("loan_btc_display", Required = Required.Always)]
        public decimal LoanBtc { get; private set; }
    }
}

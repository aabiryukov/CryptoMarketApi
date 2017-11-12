using Newtonsoft.Json;

namespace BTCChina
{
	public class AccountProfile
	{
		[JsonProperty("username", Required = Required.Always)]
		public string UserName { get; private set; }
		[JsonProperty("trade_password_enabled", Required = Required.Always)]
		public bool TradePasswordEnabled { get; private set; }
		[JsonProperty("otp_enabled", Required = Required.Always)]
		public bool OtpEnabled { get; private set; }
		[JsonProperty("trade_fee", Required = Required.Always)]
		public decimal TradeFee { get; private set; }
		[JsonProperty("daily_btc_limit", Required = Required.Always)]
		public int DailyBtcLimit { get; private set; }
		[JsonProperty("btc_deposit_address", Required = Required.Always)]
		public string BtcDepositAddress { get; private set; }
		[JsonProperty("btc_withdrawal_address", Required = Required.Always)]
		public string BtcWithdrawalAddress { get; private set; }
		[JsonProperty("api_key_permission", Required = Required.Always)]
		public int ApiKeyPermission { get; private set; }
	}
}

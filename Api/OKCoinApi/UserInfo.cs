using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OKCoin
{
	public class UserInfo
	{
		public decimal AssetNet { get; private set; }
		public decimal AssetTotal { get; private set; }
//		public Balance Borrow { get; private set; }
		public Balance Free { get; private set; }
		public Balance Freezed { get; private set; }
//		public Balance UnionFund { get; private set; }


		public static UserInfo ReadFromJObject(JToken o)
		{
			if (o == null)
				return null;

			var funds = o["funds"];
			return new UserInfo()
			{
				AssetNet = funds["asset"].Value<decimal>("net"),
				AssetTotal = funds["asset"].Value<decimal>("total"),

				Free = new Balance()
				{
					Btc = funds["free"].Value<decimal>("btc"),
					Usd = funds["free"].Value<decimal>("usd")
				},

				Freezed = new Balance()
				{
					Btc = funds["freezed"].Value<decimal>("btc"),
					Usd = funds["freezed"].Value<decimal>("usd")
				},
			};
		}
	}
}

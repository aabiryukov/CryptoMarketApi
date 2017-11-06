using System;
using BtcE;

namespace ApiTest
{
	static class BtceTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";

		public static void Test()
		{
/*
			{
				var btceApi = new BtceApi(ApiKey, ApiSecret);
				var info = btceApi.GetInfo();
				Console.WriteLine("Info: BTC={0}, USD={1}", info.Funds.Btc, info.Funds.Usd);
				var order = btceApi.Trade(BtcePair.btc_usd, TradeType.Sell, 1245.8M, 3M);
				// 4.801251M
				Console.WriteLine("Order: {0}", order);
			}
*/

/*
			{
				var api = new BtceApi(ApiKey, ApiSecret);

				while (true)
				{
					var time = DateTime.Now;
					var userInfo = api.GetInfo();
					var ping = Math.Round((DateTime.Now - time).TotalSeconds, 2);
					Console.WriteLine("Ping [{1}]: {0}", ping, userInfo.Funds.Btc);

					Thread.Sleep(1000);
				}
			}
*/
			{
				var ticker = BtceApi.GetTicker(BtcePair.btc_usd);
				Console.WriteLine(ticker);

				var depth3 = BtceApiV3.GetDepth(new BtcePair[] {BtcePair.btc_usd});
				Console.WriteLine(depth3);

				var ticker3 = BtceApiV3.GetTicker(new BtcePair[] {BtcePair.btc_usd});
				var trades3 = BtceApiV3.GetTrades(new BtcePair[] {BtcePair.btc_usd});
				var trades = BtceApi.GetTrades(BtcePair.btc_usd);
				var btcusdDepth = BtceApi.GetDepth(BtcePair.usd_rur);
				var fee = BtceApi.GetFee(BtcePair.usd_rur);

				var btceApi = new BtceApi(ApiKey, ApiSecret);
				var info = btceApi.GetInfo();
				Console.WriteLine("Info: BTC={0}, USD={1}", info.Funds.Btc, info.Funds.Usd);
				var transHistory = btceApi.GetTransHistory(since: DateTime.Now.AddMonths(-6));
				Console.WriteLine("transHistory count={0}", transHistory.Count);
				var tradeHistory = btceApi.GetTradeHistory(count: 20);
				Console.WriteLine(tradeHistory);
				var orderList = btceApi.ActiveOrders();
				Console.WriteLine("OrderList:");
				foreach (var or in orderList)
				{
					Console.WriteLine("{0}: {1}", or.Key, or.Value);
				}
//			var tradeAnswer = btceApi.Trade(BtcePair.btc_usd, TradeType.Sell, 20, 0.1m);
//			var cancelAnswer = btceApi.CancelOrder(tradeAnswer.OrderId);
			}
		}
	}
}

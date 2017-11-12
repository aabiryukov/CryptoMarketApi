using System;
using Wex;

namespace ApiTest
{
	static class WexTest
	{
//		private const string ApiKey = "PUT-HERE-API-KEY";
//		private const string ApiSecret = "PUT-HERE-API-SECRET";
        private const string ApiKey = "FJ6567VM-GMWFTOPI-26H58S3U-ST3TVQJE-7V81IVPK";
        private const string ApiSecret = "e7673dff7525d504428c8de560e95940f14db3790dad4d947d6f9edd62fc5eed";

        public static void Test()
		{
/*
			{
				var apiClient = new WexApi(ApiKey, ApiSecret);
				var info = apiClient.GetInfo();
				Console.WriteLine("Info: BTC={0}, USD={1}", info.Funds.Btc, info.Funds.Usd);
				var order = apiClient.Trade(BtcePair.btc_usd, TradeType.Sell, 1245.8M, 3M);
				// 4.801251M
				Console.WriteLine("Order: {0}", order);
			}
*/

/*
			{
				var apiClient = new WexApi(ApiKey, ApiSecret);

				while (true)
				{
					var time = DateTime.Now;
					var userInfo = apiClient.GetInfo();
					var ping = Math.Round((DateTime.Now - time).TotalSeconds, 2);
					Console.WriteLine("Ping [{1}]: {0}", ping, userInfo.Funds.Btc);

					Thread.Sleep(1000);
				}
			}
*/
			{
				var ticker = WexApi.GetTicker(WexPair.btc_usd);
				Console.WriteLine(ticker);

				var depth3 = WexApiV3.GetDepth(new WexPair[] {WexPair.btc_usd});
				Console.WriteLine(depth3);

				var ticker3 = WexApiV3.GetTicker(new WexPair[] {WexPair.btc_usd});
				var trades3 = WexApiV3.GetTrades(new WexPair[] {WexPair.btc_usd});
				var trades = WexApi.GetTrades(WexPair.btc_usd);
				var btcusdDepth = WexApi.GetDepth(WexPair.usd_rur);
				var fee = WexApi.GetFee(WexPair.usd_rur);

				var apiClient = new WexApi(ApiKey, ApiSecret);
				var info = apiClient.GetInfo();
				Console.WriteLine("Info: BTC={0}, USD={1}", info.Funds.Btc, info.Funds.Usd);
				var transHistory = apiClient.GetTransHistory(since: DateTime.Now.AddMonths(-6));
				Console.WriteLine("transHistory count={0}", transHistory.Count);
				var tradeHistory = apiClient.GetTradeHistory(count: 20);
				Console.WriteLine(tradeHistory);
				var orderList = apiClient.ActiveOrders();
				Console.WriteLine("OrderList:");
				foreach (var or in orderList)
				{
					Console.WriteLine("{0}: {1}", or.Key, or.Value);
				}

//			var tradeAnswer = apiClient.Trade(BtcePair.btc_usd, TradeType.Sell, 20, 0.1m);
//			var cancelAnswer = apiClient.CancelOrder(tradeAnswer.OrderId);
			}
		}
	}
}

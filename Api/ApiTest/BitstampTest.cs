using System;
using System.Linq;
using Bitstamp;

namespace ApiTest
{
	static class BitstampTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";
		private const string ClientId = "PUT-HERE-CLIENT-ID";

		public static void Test()
		{
			var ticker = BitstampApi.GetTicker();
			Console.WriteLine(ticker.Last);

			var trans = BitstampApi.GetTransactions();
			Console.WriteLine("trans.Count=" + trans.Count);
			var orderBook = BitstampApi.GetOrderBook();
			Console.WriteLine("orderBook.Asks.Count=" + orderBook.Asks.Count);

			var api = new BitstampApi(ApiKey, ApiSecret, ClientId);

			var balance = api.GetBalance();
			Console.WriteLine(balance);

			var openOrders = api.GetOpenOrders();
			Console.WriteLine("Open orders: {0}", openOrders.Count());

			var cancelResult = api.CancelOrder(12345);
			Console.WriteLine("CancelOrder: {0}", cancelResult);

			var sellAnswer = api.Sell(12456.3M, 2);
			Console.WriteLine("Sell: {0}", sellAnswer);

			var buyAnswer = api.Buy(12.3M, 1);
			Console.WriteLine("Buy: {0}", buyAnswer);

/*
			Console.WriteLine(info);
			var transHistory = btceApi.GetTransHistory();
			Console.WriteLine(transHistory);
			var tradeHistory = btceApi.GetTradeHistory(count: 20);
			Console.WriteLine(tradeHistory);
			var orderList = btceApi.ActiveOrders();
			Console.WriteLine(orderList);
//			var tradeAnswer = btceApi.Trade(BtcePair.btc_usd, TradeType.Sell, 20, 0.1m);
//			var cancelAnswer = btceApi.CancelOrder(tradeAnswer.OrderId);
*/

		}
	}
}

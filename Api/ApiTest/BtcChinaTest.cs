using System;
using System.Threading;
using BTCChina;
using BTCChina.WebSockets;

namespace ApiTest
{
	static class BtcChinaTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";

		public static void Test()
		{
/*
			// Sell
			{
			    Console.WriteLine("SALE Testing...");
				var priceUsd = 245 * 99999;
				var CIY2USD = 0.1611;

				var client1 = new BTCChinaAPI(ApiKey, ApiSecret);
				var responce = client1.PlaceOrder((double)(priceUsd / CIY2USD), -(double)2.5,
					BTCChinaAPI.MarketType.BTCCNY);
				Console.WriteLine(responce);
			}
*/

/*
			using (var wsApi = new BtcChinaWebSocketApi())
			{
                Console.WriteLine("Btcc: Socket starting...");
                wsApi.Start();
				Console.ReadLine();
				wsApi.Stop();
			}
*/

			var ticker = BTCChinaAPI.GetTicker();
			Console.WriteLine("ticker.Last: " + ticker.Last);

			var orderBook = BTCChinaAPI.GetOrderBook();
			Console.WriteLine("orderBook.Asks.Count=" + orderBook.Asks.Count);

			var client = new BTCChinaAPI(ApiKey, ApiSecret);
			var info = client.getAccountInfo();
			Console.WriteLine("UserName= " + info.Profile.UserName);

			var orderId = client.PlaceOrder(1, 1, BTCChinaAPI.MarketType.BTCCNY);
			Console.WriteLine("orderId: " + orderId);

			var cancelResult = client.cancelOrder(orderId);
			Console.WriteLine("cancelResult: " + cancelResult);
		}
	}
}

using System;
using System.Linq;
using OKCoin;

namespace ApiTest
{
// ReSharper disable once InconsistentNaming
	static class OKCoinTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";

		public static void Test()
		{
/*
			// Sell
			{
				var priceUsd = 245 * 99999;
				var CIY2USD = 0.1611;

				var client1 = new OKCoinAPI(ApiKey, ApiSecret);
				var responce = client1.PlaceOrder((double)(priceUsd / CIY2USD), -(double)2.5,
					OKCoinAPI.MarketType.BTCCNY);
				Console.WriteLine(responce);
			}
*/


			using (var wsApi = new WebSocketApi())
			{
				wsApi.OrdersDepthReceived += OnOrdersDepth;
				wsApi.Start();
				Console.ReadLine();
				wsApi.Stop();
			}

			var ticker = OKCoinAPI.GetTicker();
			Console.WriteLine("ticker.Last: " + ticker.Last);

			var orderBook = OKCoinAPI.GetOrderBook(33);
			Console.WriteLine("orderBook.Asks.Count=" + orderBook.Asks.Count);

			var client = new OKCoinAPI(ApiKey, ApiSecret);

			var info = client.GetUserInfo();
			Console.WriteLine("FreeBtc= " + info.Free.Btc);

			var orderId = client.TradeSell(300, 0.08M);
			Console.WriteLine("orderId: " + orderId);

			var orders = client.GetActiveOrders();
			Console.WriteLine("Active orders: " + orders.Count());

			var cancelResult = client.CancelOrder(orderId);
			Console.WriteLine("cancelResult: " + cancelResult);
		}

		private static void OnOrdersDepth(object sender, OrdersDepthReceivedEventArgs e)
		{
			Console.WriteLine("[OnOrdersDepth] Asks[{0}]: ({1}) Bids[{2}]: ({3})", e.OrdersDepth.Asks.Count, e.OrdersDepth.Asks[e.OrdersDepth.Asks.Count-1], e.OrdersDepth.Bids.Count, e.OrdersDepth.Bids[0]);
		}
	}
}

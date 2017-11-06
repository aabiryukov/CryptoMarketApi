using System;
using System.Linq;
using Huobi;

namespace ApiTest
{
// ReSharper disable once InconsistentNaming
	static class HuobiTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";

		public static void Test()
		{
/*
			using (var wsApi = new WebSocketApi())
			{
				wsApi.OrdersDepthReceived += OnOrdersDepth;
				wsApi.Start();
				Console.ReadLine();
				wsApi.Stop();
			}
*/
			var ticker = HuobiAPI.GetTicker();
			Console.WriteLine("ticker.Last: " + ticker.Last);

			var orderBook = HuobiAPI.GetOrderBook(33);
			Console.WriteLine("orderBook.Asks.Count=" + orderBook.Asks.Count);

			var client = new HuobiAPI(ApiKey, ApiSecret);

			var info = client.GetAccountInfo();
			Console.WriteLine("FreeBtc= " + info.AvailableBtc);

			var orderId = client.TradeSell(3000, 0.002M);
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

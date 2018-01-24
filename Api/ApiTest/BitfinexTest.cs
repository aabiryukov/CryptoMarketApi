using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex;
using Bitfinex.Json;
using Bitfinex.Json.Objects;
using Bitfinex.Json.SocketObjets;

namespace ApiTest
{
	class BitfinexTest
	{
		private const string ApiKey = "PUT-HERE-API-KEY";
		private const string ApiSecret = "PUT-HERE-API-SECRET";


		public static void Test()
		{

            using (var wsApi = new BitfinexSocketApi())
			{
                BitfinexSocketApi.SetLogVerbosity(Bitfinex.Logging.LogVerbosity.Info);

                Console.WriteLine("Bitfinex: Socket starting...");
                wsApi.Connect();

                Task.Delay(3000).Wait();
/*
                var subcribtion1 = wsApi.SubscribeToTradingPairTicker("tETHBTC", summary =>
                {
                    Console.WriteLine($"{DateTime.Now} BTC-ETH: {summary.LastPrice}");
                });
                Console.WriteLine($"Subcribtion1: {subcribtion1}");
*/

                var subcribtion2 = wsApi.SubscribeToOrderBooks("tETHBTC", OnOrderBooks, frequency: Frequency.F0, length: 1);
                Console.WriteLine($"Subcribtion2: {subcribtion2}");

                Console.ReadLine();
			}

/*
			var ticker = BitfinexApi.GetPublicTicker(BtcInfo.PairTypeEnum.btcusd, BtcInfo.BitfinexUnauthenicatedCallsEnum.pubticker);
			Console.WriteLine(ticker.LastPrice);

			var trades = BitfinexApi.GetPairTrades(BtcInfo.PairTypeEnum.btcusd, BtcInfo.BitfinexUnauthenicatedCallsEnum.trades);
			Console.WriteLine("trades.Count=" + trades.Count);

			var orderBook = BitfinexApi.GetOrderBook(BtcInfo.PairTypeEnum.btcusd);
			Console.WriteLine("orderBook.Asks.Length={0}, orderBook.Bids.Length={1}", orderBook.Asks.Length, orderBook.Bids.Length);
*/
			var api = new BitfinexApi(ApiKey, ApiSecret);

			var balances = api.GetBalances();
			var usd = balances.FirstOrDefault(x => x.Type == "exchange" && x.Currency == "usd");
			var btc = balances.FirstOrDefault(x => x.Type == "exchange" && x.Currency == "btc");
			Console.WriteLine("usd: " + usd);
			Console.WriteLine("btc: " + btc);

			foreach (var balance in balances)
			{
				Console.WriteLine("balance: " + balance);
			}

			var info = api.GetAccountInformation();
			Console.WriteLine("Account info: {0}", info);

			var openOrders = api.GetActiveOrders();
			Console.WriteLine("Open orders: {0}", openOrders.Count());
/*
			var cancelResult = api.CancelOrder(12345);
			Console.WriteLine("CancelOrder: {0}", cancelResult);

			var sellAnswer = api.Sell(12456.3M, 2);
			Console.WriteLine("Sell: {0}", sellAnswer);

			var buyAnswer = api.Buy(12.3M, 1);
			Console.WriteLine("Buy: {0}", buyAnswer);
 */ 
		}

        private static void OnOrderBooks(BitfinexSocketOrderBook[] orders)
        {
            foreach (var order in orders)
            {
                Console.WriteLine($"{DateTime.Now} | [{order.Price}] {order.Amount} : {order.Count}");
            }
        }
    }
}

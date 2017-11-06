using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Bitfinex.Json;

namespace Bitfinex
{
	public sealed class BitfinexApi
	{
		private static class WebApi
		{
			private static readonly HttpClient st_client = new HttpClient();

			static WebApi()
			{
				st_client.Timeout = TimeSpan.FromSeconds(2);
				st_client.BaseAddress = new Uri(BaseBitfinexUrl);
			}

			public static HttpClient Client { get { return st_client; } }
/*
			public static string Query(string url)
			{
				var resultString = Client.GetStringAsync(url).Result;
				return resultString;
			}
 */
			public static string GetBaseResponse(string url)
			{
				var response = Client.GetStringAsync(url).Result;
				return response;
			}

		}

// ReSharper disable once InconsistentNaming
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		private readonly string m_apiSecret;
		private readonly string m_apiKey;

		private const string ApiBfxKey = "X-BFX-APIKEY";
		private const string ApiBfxPayload = "X-BFX-PAYLOAD";
		private const string ApiBfxSig = "X-BFX-SIGNATURE";

		private const string SymbolDetailsRequestUrl = @"/v1/symbols_details";
		private const string BalanceRequestUrl = @"/v1/balances";
		private const string DepthOfBookRequestUrl = @"v1/book/";
		private const string NewOrderRequestUrl = @"/v1/order/new";
		private const string OrderStatusRequestUrl = @"/v1/order/status";
		private const string OrderCancelRequestUrl = @"/v1/order/cancel";
		private const string CancelAllRequestUrl = @"/all";
		private const string CancelReplaceRequestUrl = @"/replace";
		private const string MultipleRequestUrl = @"/multi";

		private const string ActiveOrdersRequestUrl = @"/v1/orders";
		private const string ActivePositionsRequestUrl = @"/v1/positions";
		private const string HistoryRequestUrl = @"/v1/history";
		private const string MyTradesRequestUrl = @"/v1/mytrades";

		private const string LendbookRequestUrl = @"/v1/lendbook/";
		private const string LendsRequestUrl = @"/v1/lends/";

		private const string DepositRequestUrl = @"/v1/deposit/new";
		private const string AccountInfoRequestUrl = @"/v1/account_infos";
		private const string MarginInfoRequstUrl = @"/v1/margin_infos";

		private const string NewOfferRequestUrl = @"/v1/offer/new";
		private const string CancelOfferRequestUrl = @"/v1/offer/cancel";
		private const string OfferStatusRequestUrl = @"/v1/offer/status";

		private const string ActiveOffersRequestUrl = @"/v1/offers";
		private const string ActiveCreditsRequestUrl = @"/v1/credits";

		private const string ActiveMarginSwapsRequestUrl = @"/v1/taken_swaps";
		private const string CloseSwapRequestUrl = @"/v1/swap/close";
		private const string ClaimPosRequestUrl = @"/v1/position/claim";

		private const string DefaulOrderExchangeType = "bitfinex";
		private const string DefaultLimitType = "exchange limit";
		private const string Buy = "buy";
		private const string Sell = "sell";

		public const string BaseBitfinexUrl = @"https://api.bitfinex.com";

		public BitfinexApi(string apiKey, string apiSecret)
		{
			m_apiKey = apiKey;
			m_apiSecret = apiSecret;
			Log.Info("Connecting to Bitfinex Api with key: {0}", apiKey);
		}

		#region Unauthenticated Calls
		public static BitfinexOrderBookGet GetOrderBook(BtcInfo.PairTypeEnum pairType, int limitBids = 30, int limitAsks = 30)
		{
			try
			{
				var url = DepthOfBookRequestUrl + Enum.GetName(typeof(BtcInfo.PairTypeEnum), pairType);
				var response = WebApi.GetBaseResponse(url + string.Format("?limit_bids={0};limit_asks={1}", limitBids, limitAsks));
				var orderBookResponseObj = JsonConvert.DeserializeObject<BitfinexOrderBookGet>(response);
				return orderBookResponseObj;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return new BitfinexOrderBookGet();
			}
		}

		public static IList<BitfinexSymbolDetailsResponse> GetSymbols()
		{
			const string url = SymbolDetailsRequestUrl;
			var response = WebApi.GetBaseResponse(url);
			var symbolsResponseObj = JsonConvert.DeserializeObject<IList<BitfinexSymbolDetailsResponse>>(response);

			foreach (var bitfinexSymbolDetailsResponse in symbolsResponseObj)
				Log.Info("Symbol: {0}", bitfinexSymbolDetailsResponse);

			return symbolsResponseObj;
		}

		public static BitfinexPublicTickerGet GetPublicTicker(BtcInfo.PairTypeEnum pairType, BtcInfo.BitfinexUnauthenicatedCallsEnum callType)
		{
			var call = Enum.GetName(typeof(BtcInfo.BitfinexUnauthenicatedCallsEnum), callType);
			var symbol = Enum.GetName(typeof(BtcInfo.PairTypeEnum), pairType);
			var url = @"/v1/" + call.ToLower(CultureInfo.InvariantCulture) + "/" + symbol.ToLower(CultureInfo.InvariantCulture);
			var response = WebApi.GetBaseResponse(url);

			var publicticketResponseObj = JsonConvert.DeserializeObject<BitfinexPublicTickerGet>(response);
			Log.Info("Ticker: {0}", publicticketResponseObj);

			return publicticketResponseObj;
		}

		public static IList<BitfinexSymbolStatsResponse> GetPairStats(BtcInfo.PairTypeEnum pairType, BtcInfo.BitfinexUnauthenicatedCallsEnum callType)
		{
			var call = Enum.GetName(typeof(BtcInfo.BitfinexUnauthenicatedCallsEnum), callType);
			var symbol = Enum.GetName(typeof(BtcInfo.PairTypeEnum), pairType);
			var url = @"/v1/" + call.ToLower(CultureInfo.InvariantCulture) + "/" + symbol.ToLower(CultureInfo.InvariantCulture);
			var response = WebApi.GetBaseResponse(url);

			var symbolStatsResponseObj = JsonConvert.DeserializeObject<IList<BitfinexSymbolStatsResponse>>(response);

			foreach (var symbolStatsResponse in symbolStatsResponseObj)
				Log.Info("Pair Stats: {0}", symbolStatsResponse);

			return symbolStatsResponseObj;
		}

		public static IList<BitfinexTradesGet> GetPairTrades(BtcInfo.PairTypeEnum pairType, BtcInfo.BitfinexUnauthenicatedCallsEnum callType)
		{
			var call = Enum.GetName(typeof(BtcInfo.BitfinexUnauthenicatedCallsEnum), callType);
			var symbol = Enum.GetName(typeof(BtcInfo.PairTypeEnum), pairType);
			var url = @"/v1/" + call.ToLower(CultureInfo.InvariantCulture) + "/" + symbol.ToLower(CultureInfo.InvariantCulture);
			var response = WebApi.GetBaseResponse(url);

			var pairTradesResponseObj = JsonConvert.DeserializeObject<IList<BitfinexTradesGet>>(response);

			foreach (var pairTrade in pairTradesResponseObj)
				Log.Info("Pair Trade: {0}", pairTrade);

			return pairTradesResponseObj;
		}

		/// <summary>
		/// symbol = ExchangeSymbolEnum
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static IList<BitfinexLendsResponse> GetLends(string symbol)
		{
			var url = LendsRequestUrl + symbol;
			var response = WebApi.GetBaseResponse(url);

			var lendResponseObj = JsonConvert.DeserializeObject<IList<BitfinexLendsResponse>>(response);
			return lendResponseObj;
		}

		public static BitfinexLendbookResponse GetLendbook(string symbol)
		{
			var url = LendbookRequestUrl + symbol;
			var response = WebApi.GetBaseResponse(url);

			var lendBookResponseObj = JsonConvert.DeserializeObject<BitfinexLendbookResponse>(response);
			return lendBookResponseObj;
		}

		#endregion

		#region Sending Crypto Orders

		public BitfinexMultipleNewOrderResponse SendMultipleOrders(BitfinexNewOrderPost[] orders)
		{
			try
			{
				var multipleOrdersPost = new BitfinexMultipleNewOrdersPost
				{
					Request = NewOrderRequestUrl + MultipleRequestUrl,
					Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
					Orders = orders
				};

				// var client = GetRestClient(multipleOrdersPost.Request);
				var response = GetRestResponse(multipleOrdersPost);

				var multipleOrderResponseObj = JsonConvert.DeserializeObject<BitfinexMultipleNewOrderResponse>(response);

				Log.Info("Sending Multiple Orders:");
				foreach (var order in orders)
					Log.Info(order.ToString());

				return multipleOrderResponseObj;

			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return null;
			}
		}

		public BitfinexNewOrderResponse SendOrder(BitfinexNewOrderPost newOrder)
		{
			try
			{
				newOrder.Request = NewOrderRequestUrl;
				newOrder.Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture);

//				var client = GetRestClient(NewOrderRequestUrl);
				var response = GetRestResponse(newOrder);

				var newOrderResponseObj = JsonConvert.DeserializeObject<BitfinexNewOrderResponse>(response);

				Log.Info("Sending New Order: {0}", newOrder.ToString());
				Log.Info("Response from Exchange: {0}", newOrderResponseObj);

				return newOrderResponseObj;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return null;
			}
		}

		public BitfinexNewOrderResponse SendOrder(string symbol, string amount, string price, string exchange, string side, string type, bool isHidden)
		{
			var newOrder = new BitfinexNewOrderPost
			{
				Symbol = symbol,
				Amount = amount,
				Price = price,
				Exchange = exchange,
				Side = side,
				Type = type//,
				//IsHidden = isHidden.ToString()
			};
			return SendOrder(newOrder);
		}

		public BitfinexNewOrderResponse SendSimpleLimit(string symbol, string amount, string price, string side, bool isHidden = false)
		{
			return SendOrder(symbol, amount, price, DefaulOrderExchangeType, side, DefaultLimitType, isHidden);
		}

		public BitfinexNewOrderResponse SendSimpleLimitBuy(string symbol, string amount, string price, bool isHidden = false)
		{
			return SendOrder(symbol, amount, price, DefaulOrderExchangeType, Buy, DefaultLimitType, isHidden);
		}

		public BitfinexNewOrderResponse SendSimpleLimitSell(string symbol, string amount, string price, bool isHidden = false)
		{
			return SendOrder(symbol, amount, price, DefaulOrderExchangeType, Sell, DefaultLimitType, isHidden);
		}

		#endregion

		#region Cancel Crypto Orders

		public BitfinexOrderStatusResponse CancelOrder(int orderId)
		{
			var cancelPost = new BitfinexOrderStatusPost
			{
				Request = OrderCancelRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				OrderId = orderId
			};

			var response = GetRestResponse(cancelPost);
			var orderCancelResponseObj = JsonConvert.DeserializeObject<BitfinexOrderStatusResponse>(response);

			Log.Info("Cancel OrderId: {0}, Response From Exchange: {1}", orderId, orderCancelResponseObj);

			return orderCancelResponseObj;
		}

		public BitfinexCancelReplaceOrderResponse CancelReplaceOrder(int cancelOrderId, BitfinexNewOrderPost newOrder)
		{
			var replaceOrder = new BitfinexCancelReplacePost
			{
				Amount = newOrder.Amount,
				CancelOrderId = cancelOrderId,
				Exchange = newOrder.Exchange,
				Price = newOrder.Price,
				Side = newOrder.Side,
				Symbol = newOrder.Symbol,
				Type = newOrder.Type
			};
			return CancelReplaceOrder(replaceOrder);
		}

		public BitfinexCancelReplaceOrderResponse CancelReplaceOrder(BitfinexCancelReplacePost replaceOrder)
		{
			replaceOrder.Request = OrderCancelRequestUrl + CancelReplaceRequestUrl;
			replaceOrder.Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture);

			var response = GetRestResponse(replaceOrder);

			var replaceOrderResponseObj = JsonConvert.DeserializeObject<BitfinexCancelReplaceOrderResponse>(response);
			replaceOrderResponseObj.OriginalOrderId = replaceOrder.CancelOrderId;

			Log.Info("Cancel Replace: {0}");
			Log.Info("Response From Exchange: {0}", replaceOrderResponseObj.ToString());

			return replaceOrderResponseObj;
		}

		public string CancelMultipleOrders(int[] intArr)
		{
			var cancelMultiplePost = new BitfinexCancelMultipleOrderPost
			{
				Request = OrderCancelRequestUrl + MultipleRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				OrderIds = intArr
			};

			var response = GetRestResponse(cancelMultiplePost);

			var str = new StringBuilder();

			foreach (var cancelOrderId in intArr)
				str.Append(cancelOrderId + ", ");

			Log.Info("Cancelling the following orders: {0}", str.ToString());
			Log.Info("Response From Exchange: {0}", response);

			return response;
		}

		public string CancellAllActiveOrders()
		{
			const string url = OrderCancelRequestUrl + CancelAllRequestUrl;
			var cancelAllPost = new BitfinexPostBase
			{
				Request = url,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(cancelAllPost);

			return response;
		}

		#endregion

		#region Trading Info
		public IList<BitfinexMarginPositionResponse> GetActiveOrders()
		{
			var activeOrdersPost = new BitfinexPostBase
			{
				Request = ActiveOrdersRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(activeOrdersPost);
			var activeOrdersResponseObj = JsonConvert.DeserializeObject<IList<BitfinexMarginPositionResponse>>(response);

			Log.Info("Active Orders:");
			foreach (var activeOrder in activeOrdersResponseObj)
				Log.Info("Order: {0}", activeOrder.ToString());

			return activeOrdersResponseObj;
		}

		public IList<BitfinexHistoryResponse> GetHistory(string currency, string since, string until, int limit, string wallet)
		{
			var historyPost = new BitfinexHistoryPost
			{
				Request = HistoryRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				Currency = currency,
				Since = since,
				Until = until,
				Limit = limit,
				Wallet = wallet
			};

			var response = GetRestResponse(historyPost);
			var historyResponseObj = JsonConvert.DeserializeObject<IList<BitfinexHistoryResponse>>(response);

			Log.Info("History:");
			foreach (var history in historyResponseObj)
				Log.Info("{0}", history);

			return historyResponseObj;
		}

		public IList<BitfinexMyTradesResponse> GetMyTrades(string symbol, string timestamp, int limit)
		{
			var myTradesPost = new BitfinexMyTradesPost
			{
				Request = MyTradesRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				Symbol = symbol,
				Timestamp = timestamp,
				Limit = limit
			};

			var response = GetRestResponse(myTradesPost);
			var myTradesResponseObj = JsonConvert.DeserializeObject<IList<BitfinexMyTradesResponse>>(response);

			Log.Info("My Trades:");
			foreach (var myTrade in myTradesResponseObj)
				Log.Info("Trade: {0}", myTrade);

			return myTradesResponseObj;
		}

		public BitfinexOrderStatusResponse GetOrderStatus(int orderId)
		{
			var orderStatusPost = new BitfinexOrderStatusPost
			{
				Request = OrderStatusRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				OrderId = orderId
			};

			var response = GetRestResponse(orderStatusPost);
			var orderStatusResponseObj = JsonConvert.DeserializeObject<BitfinexOrderStatusResponse>(response);

			Log.Info("OrderId: {0} Status: {1}", orderId, orderStatusResponseObj);

			return orderStatusResponseObj;
		}

		#endregion

		#region Account Information

		public IList<BitfinexBalanceResponse> GetBalances()
		{
			var balancePost = new BitfinexPostBase
			{
				Request = BalanceRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(balancePost);
			var balancesObj = JsonConvert.DeserializeObject<IList<BitfinexBalanceResponse>>(response);

			Log.Info("Balances:");
			foreach (var balance in balancesObj)
				Log.Info(balance);

			return balancesObj;
		}


		/// <summary>
		/// currency = upper case ExchangeSymbolEnum
		/// method = lower case ExchangeSymbolNameEnum
		/// wallet = BitfinexWalletEnum
		/// </summary>
		/// <param name="currency"></param>
		/// <param name="method"></param>
		/// <param name="wallet"></param>
		/// <returns></returns>
		public BitfinexDepositResponse Deposit(string currency, string method, string wallet)
		{
			var depositPost = new BitfinexDepositPost
			{
				Request = DepositRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				Currency = currency,
				Method = method,
				WalletName = wallet
			};

			// var client = GetRestClient(depositPost.Request);
			var response = GetRestResponse(depositPost);

			var depositResponseObj = JsonConvert.DeserializeObject<BitfinexDepositResponse>(response);
			Log.Info("Attempting to deposit: {0} with method: {1} to wallet: {2}", currency, method, wallet);
			Log.Info("Response from exchange: {0}", depositResponseObj);
			return depositResponseObj;
		}

		/// <summary>
		/// This never worked for me...
		/// </summary>
		/// <returns></returns>
		public object GetAccountInformation()
		{
			var accountPost = new BitfinexPostBase
			{
				Request = AccountInfoRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(accountPost);
			Log.Info("Account Information: {0}", response);
			return response;
		}

		public BitfinexMarginInfoResponse GetMarginInformation()
		{
			var marginPost = new BitfinexPostBase
			{
				Request = MarginInfoRequstUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(marginPost);

			var jArr = JsonConvert.DeserializeObject(response) as JArray;
			if (jArr == null || jArr.Count == 0)
				return null;

			var marginInfoStr = jArr[0].ToString();
			var marginInfoResponseObj = JsonConvert.DeserializeObject<BitfinexMarginInfoResponse>(marginInfoStr);

			Log.Info("Margin Info: {0}", marginInfoResponseObj.ToString());

			return marginInfoResponseObj;
		}

		public IList<BitfinexMarginPositionResponse> GetActivePositions()
		{
			var activePositionsPost = new BitfinexPostBase
			{
				Request = ActivePositionsRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(activePositionsPost);

			var activePositionsResponseObj = JsonConvert.DeserializeObject<IList<BitfinexMarginPositionResponse>>(response);

			Log.Info("Active Positions: ");
			foreach (var activePos in activePositionsResponseObj)
				Log.Info("Position: {0}", activePos);

			return activePositionsResponseObj;
		}

		#endregion

		#region Lending and Borrowing Execution

		/// <summary>
		/// rate is the yearly rate. So if you want to borrow/lend at 10 basis points per day you would 
		/// pass in 36.5 as the rate (10 * 365). Also, lend = lend (aka offer swap), loan = borrow (aka receive swap)
		/// The newOffer's currency propery = ExchangeSymbolEnum uppercase.
		/// </summary>
		/// <param name="newOffer"></param>
		/// <returns></returns>
		public BitfinexOfferStatusResponse SendNewOffer(BitfinexNewOfferPost newOffer)
		{
			newOffer.Request = NewOfferRequestUrl;
			newOffer.Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture);

			var response = GetRestResponse(newOffer);

			var newOfferResponseObj = JsonConvert.DeserializeObject<BitfinexOfferStatusResponse>(response);

			Log.Info("Sending New Offer: {0}", newOffer.ToString());
			Log.Info("Response From Exchange: {0}", newOfferResponseObj);
			return newOfferResponseObj;
		}

		/// <summary>
		/// rate is the yearly rate. So if you want to borrow/lend at 10 basis points per day you would 
		/// pass in 36.5 as the rate (10 * 365). Also, lend = lend (aka offer swap), loan = borrow (aka receive swap)
		/// </summary>
		/// <param name="currency"></param>
		/// <param name="amount"></param>
		/// <param name="rate"></param>
		/// <param name="period"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public BitfinexOfferStatusResponse SendNewOffer(string currency, string amount, string rate, int period, string direction)
		{
			var newOffer = new BitfinexNewOfferPost
			{
				Amount = amount,
				Currency = currency,
				Rate = rate,
				Period = period,
				Direction = direction
			};
			return SendNewOffer(newOffer);
		}

		/// <summary>
		/// Note: bug with bitfinex Canceloffer - the object returned will still say offer is alive and not cancelled.
		/// If you execute a 'GetOfferStatus' after the cancel is alive will be true (aka the offer will show up as cancelled. 
		/// </summary>
		/// <param name="offerId"></param>
		/// <returns></returns>
		public BitfinexOfferStatusResponse CancelOffer(int offerId)
		{
			var cancelPost = new BitfinexOfferStatusPost
			{
				Request = CancelOfferRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				OfferId = offerId
			};

			var response = GetRestResponse(cancelPost);
			var orderCancelResponseObj = JsonConvert.DeserializeObject<BitfinexOfferStatusResponse>(response);

			Log.Info("Cancelling offerId: {0}. Exchange response: {1}", offerId, orderCancelResponseObj);

			return orderCancelResponseObj;
		}

		public BitfinexOfferStatusResponse GetOfferStatus(int offerId)
		{
			var statusPost = new BitfinexOfferStatusPost
			{
				Request = OfferStatusRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				OfferId = offerId
			};

			var response = GetRestResponse(statusPost);
			var offerStatuslResponseObj = JsonConvert.DeserializeObject<BitfinexOfferStatusResponse>(response);

			Log.Info("Status of offerId: {0}. Exchange response: {1}", offerId, offerStatuslResponseObj);

			return offerStatuslResponseObj;
		}

		public IList<BitfinexOfferStatusResponse> GetActiveOffers()
		{
			var activeOffersPost = new BitfinexPostBase
			{
				Request = ActiveOffersRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(activeOffersPost);
			var activeOffersResponseObj = JsonConvert.DeserializeObject<IList<BitfinexOfferStatusResponse>>(response);

			Log.Info("Active Offers:");
			foreach (var activeOffer in activeOffersResponseObj)
				Log.Info("Offer: {0}", activeOffer.ToString());

			return activeOffersResponseObj;
		}

		public IList<BitfinexActiveCreditsResponse> GetActiveCredits()
		{
			var activeCreditsPost = new BitfinexPostBase
			{
				Request = ActiveCreditsRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(activeCreditsPost);
			var activeCreditsResponseObj = JsonConvert.DeserializeObject<IList<BitfinexActiveCreditsResponse>>(response);

			Log.Info("Active Credits:");
			foreach (var activeCredits in activeCreditsResponseObj)
				Log.Info("Credits: {0}", activeCredits.ToString());

			return activeCreditsResponseObj;
		}

		/// <summary>
		/// In the Total Return Swaps page you will see a horizontal header "Swaps used in margin position"
		/// This function returns information about what you have borrowed. If you want to close the 
		/// swap you must pass the id returned here to the "CloseSwap" function. 
		/// If you want to 'cash out' and claim the position you must pass the position id to the "ClaimPosition" function. 
		/// </summary>
		/// <returns></returns>
		public IList<BitfinexActiveSwapsInMarginResponse> GetActiveSwapsUsedInMarginPosition()
		{
			var activeSwapsInMarginPost = new BitfinexPostBase
			{
				Request = ActiveMarginSwapsRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture)
			};

			var response = GetRestResponse(activeSwapsInMarginPost);
			var activeSwapsInMarginResponseObj = JsonConvert.DeserializeObject<IList<BitfinexActiveSwapsInMarginResponse>>(response);

			Log.Info("Active Swaps In Margin Pos:");
			foreach (var activeSwaps in activeSwapsInMarginResponseObj)
				Log.Info("Swaps used in margin: {0}", activeSwaps.ToString());

			return activeSwapsInMarginResponseObj;
		}

		public BitfinexActiveSwapsInMarginResponse CloseSwap(int swapId)
		{
			var closeSwapPost = new BitfinexCloseSwapPost
			{
				Request = CloseSwapRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				SwapId = swapId
			};

			var response = GetRestResponse(closeSwapPost);
			var closeSwapResponseObj = JsonConvert.DeserializeObject<BitfinexActiveSwapsInMarginResponse>(response);

			Log.Info("Close Swap Id: {0}, Response from Exchange: {1}", swapId, closeSwapResponseObj);

			return closeSwapResponseObj;
		}

		/// <summary>
		/// Ok... so from what I gather is:
		/// If you leverage usd for btc, and the price moved in your favor the trade
		/// you can physically claim the btc in your wallet as yours. You will notice the
		/// object return this function is the same as the GetActiveSwapUsedInMarginPosition
		/// </summary>
		/// <param name="positionId"></param>
		/// <returns></returns>
		public BitfinexMarginPositionResponse ClaimPosition(int positionId)
		{
			var claimPosPost = new BitfinexClaimPositionPost
			{
				Request = ClaimPosRequestUrl,
				Nonce = Common.UnixTimeStampUtc().ToString(CultureInfo.InvariantCulture),
				PositionId = positionId
			};

			var response = GetRestResponse(claimPosPost);
			var claimPosResponseObj = JsonConvert.DeserializeObject<BitfinexMarginPositionResponse>(response);

			Log.Info("Claim Position Id: {0}, Response from Exchange: {1}", positionId, claimPosResponseObj);

			return claimPosResponseObj;
		}

		#endregion

		#region RestCalls

		private string GetRestResponse(BitfinexPostBase obj)
		{
			HttpRequestMessage request;
			{
				var jsonObj = JsonConvert.SerializeObject(obj);
				var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonObj));
				request = new HttpRequestMessage(HttpMethod.Post, obj.Request);
				request.Headers.Add(ApiBfxKey, m_apiKey);
				request.Headers.Add(ApiBfxPayload, payload);
				request.Headers.Add(ApiBfxSig, GetHexHashSignature(payload));
			}

			var responseMessage = WebApi.Client.SendAsync(request).Result;
			var response = responseMessage.Content.ReadAsStringAsync().Result;

			CheckResultCode(responseMessage.StatusCode, response);

			return response;
		}

		private static void CheckResultCode(HttpStatusCode statusCode, string response)
		{
			if (statusCode == HttpStatusCode.OK)
				return;
	
			var error = JsonConvert.DeserializeObject<ErrorResponse>(response);
			if (error != null && error.Message != null)
			{
				throw new ApplicationException("Bitfinex error: " + error.Message);
			}

			throw new ApplicationException("Bitfinex error: StatusCode = " + statusCode);
		}

		#endregion

//		private void CheckResultCode()

		private string GetHexHashSignature(string payload)
		{
			using (var hmac = new HMACSHA384(Encoding.UTF8.GetBytes(m_apiSecret)))
			{
				var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
				return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
			}
		}

	}
}

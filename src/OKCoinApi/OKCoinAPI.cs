using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
//using System.Runtime.Serialization.Json;//for NET 3.5, assembly System.ServiceModel.Web; for NET 4.0+, assembly System.Runtime.Serialization
//using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace OKCoin
{
// ReSharper disable once InconsistentNaming
    public sealed class OKCoinAPI
    {
		private static class WebApi
		{
			private static readonly HttpClient st_client = new HttpClient
			{
				BaseAddress = new Uri("https://www.okcoin.com/api/v1/"),
				Timeout = TimeSpan.FromSeconds(2)
			};

			internal static HttpClient Client { get { return st_client; } }

			public static string Query(string url)
			{
				try
				{
					var resultString = Client.GetStringAsync(url).Result;
					return resultString;
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception.ToString());
					throw;
				}
			}
		}

		private class NameValueDictionary : SortedDictionary<string, string>{}

        private readonly string m_accessKey;
	    private readonly string m_secretKey;
	    //constants
//        private const int BTCChinaConnectionLimit = 1;


		public static Ticker GetTicker()
		{
			const string queryStr = "ticker.do?ticker.do?symbol=btc_usd";
			var response = WebApi.Query(queryStr);
			var jobj = JObject.Parse( response );
			return Ticker.ReadFromJObject(jobj);
		}

	    public static OrderBook GetOrderBook()
	    {
		    return GetOrderBook(20);
	    }

		public static OrderBook GetOrderBook(int limit)
		{
			var queryStr = string.Format(CultureInfo.InvariantCulture, "depth.do?symbol=btc_usd&size={0}", limit);
			var json = WebApi.Query(queryStr);
			var book = JsonConvert.DeserializeObject<OrderBook>(json);
			return book;
		}

        /// <summary>
        /// Unique ctor sets access key and secret key, which cannot be changed later.
        /// </summary>
        /// <param name="accessKey">Your Access Key</param>
        /// <param name="secretKey">Your Secret Key</param>
        public OKCoinAPI(string accessKey, string secretKey)
        {
            m_accessKey = accessKey;
            m_secretKey = secretKey;
            // HttpWebRequest setups
//            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; //for https
//            ServicePointManager.DefaultConnectionLimit = 1; //one concurrent connection is allowed for this server atm.
            //ServicePointManager.UseNagleAlgorithm = false;
        }

		/// <summary>
		/// Place sell order
		/// </summary>
		/// <param name="price">Order price. For limit orders, the price must be between 0~1,000,000. IMPORTANT: for market buy orders, the price is to total amount you want to buy, and it must be higher than the current price of 0.01 BTC (minimum buying unit) or 0.1 LTC.</param>
		/// <param name="amount">The amount of BTC to sell</param>
		/// <returns>Order id</returns>
	    public int TradeSell(decimal price, decimal amount)
	    {
		    return InternalTrade(price, amount, true);
		}

		/// <summary>
		/// Place buy order
		/// </summary>
		/// <param name="price">Order price. For limit orders, the price must be between 0~1,000,000. IMPORTANT: for market buy orders, the price is to total amount you want to buy, and it must be higher than the current price of 0.01 BTC (minimum buying unit) or 0.1 LTC.</param>
		/// <param name="amount">Order quantity. Must be higher than 0.01 for BTC, or 0.1 for LTC.</param>
		/// <returns>Order id</returns>
		public int TradeBuy(decimal price, decimal amount)
		{
			return InternalTrade(price, amount, false);
		}

	    private int InternalTrade(decimal price, decimal amount, bool isSell)
	    {
			var args = new NameValueCollection
	        {
		        {"symbol", "btc_usd"},
		        {"type", isSell ? "sell" : "buy"},
				{"price", price.ToString(CultureInfo.InvariantCulture)},
				{"amount", amount.ToString(CultureInfo.InvariantCulture)}
	        };

			var response = DoMethod2("trade", args);
			var jsonObject = JObject.Parse(response);
			var success = jsonObject.Value<bool>("result");
		    if (!success)
		    {
			    var errorCode = jsonObject.Value<string>("error_code");
				throw new OKCoinException("InternalTrade", "Failed to create order: " + (isSell ? "Sell" : "Buy") + ". ErrorCode=" + errorCode);
		    }

		    var orderId = jsonObject.Value<int>("order_id");

			return orderId;
		}

		public IEnumerable<Order> GetActiveOrders()
		{
			var args = new NameValueCollection
	        {
		        {"symbol", "btc_usd"},
		        {"order_id", "-1"}
	        };

			var response = DoMethod2("order_info", args);
			
			var jsonObject = CheckResult(response);
			var orders = jsonObject["orders"].ToObject<List<Order>>();

			return orders;
		}

	    private static JObject CheckResult(string response)
	    {
			var jsonObject = JObject.Parse(response);
			var success = jsonObject.Value<bool>("result");
			if (!success)
			{
				var errorCode = jsonObject.Value<string>("error_code");
				throw new OKCoinException("CheckResult", "Failed OKCoin request. ErrorCode=" + errorCode);
			}

		    return jsonObject;
	    }

	    /// <summary>
        /// Cancel an active order if the status is 'open'.
        /// </summary>
        /// <param name="orderId">The order id to cancel.</param>
        /// <returns>true or false depending on the result of cancellation.</returns>
        public bool CancelOrder(int orderId)
        {
	        var args = new NameValueCollection
	        {
		        {"symbol", "btc_usd"},
		        {"order_id", orderId.ToString(CultureInfo.InvariantCulture)}
	        };

	        var response = DoMethod2("cancel_order", args);
			var jsonObject = JObject.Parse(response);
			var success = jsonObject.Value<bool>("result");

	        return success;
        }

        /// <summary>
        /// Get stored account information and user balance.
        /// </summary>
        /// <returns>objects profile, balance and frozen in JSON</returns>
        public UserInfo GetUserInfo()
        {
			var response = DoMethod2("userinfo", null);
			var jobj = JObject.Parse(response);

	        var result = jobj.Value<bool>("result");
			if(!result)
				throw new OKCoinException("GetUserInfo", "Request failed");

			var userInfo = UserInfo.ReadFromJObject(jobj["info"]);
	        return userInfo;
        }

		private string DoMethod2(string method, NameValueCollection jParams)
		{
			var args = new NameValueDictionary();

			args.Add("api_key", m_accessKey);

			if (jParams != null)
			{
				foreach (var key in jParams.AllKeys)
				{
					args.Add(key, jParams[key]);
				}
			}

			var sign = GetSignature(args, m_secretKey);
			args.Add("sign", sign);

			var httpContent = new FormUrlEncodedContent(args);
			var response = WebApi.Client.PostAsync(method + ".do", httpContent).Result;
			var resultString = response.Content.ReadAsStringAsync().Result;
			return resultString;
		}

	    private static string GetSignature(NameValueDictionary args, string secretKey)
	    {
		    var sb = new StringBuilder();
			foreach (var arg in args)
			{
				sb.Append(arg.Key);
				sb.Append("=");
				sb.Append(arg.Value);
				sb.Append("&");
			}

			sb.Append("secret_key=");
		    sb.Append(secretKey);

		    using (var md = MD5.Create())
		    {
				using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString())))
		        {
			        var hashData = md.ComputeHash(stream);

					// Format as hexadecimal string.
					var hashBuilder = new StringBuilder();
					foreach (byte data in hashData)
					{
						hashBuilder.Append(data.ToString("x2", CultureInfo.InvariantCulture));
					}
					return hashBuilder.ToString().ToUpperInvariant();
		        }
		    }
	    }
    }
}

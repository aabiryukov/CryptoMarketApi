using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Huobi
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Huobi API.
    /// <![CDATA[
    /// Help: https://www.huobi.com/help/index.php?a=api_help_v3&lang=en#con_3
    /// ]]>
    /// </summary>
    public sealed class HuobiAPI
    {
        private enum CoinType
        {
            Btc = 1,
            Ltc = 2
        }

#pragma warning disable 0649
        // ReSharper disable once ClassNeverInstantiated.Local
        private class HuobiOrderResult
        {
            [JsonProperty("result", Required = Required.Always)]
            public string Result;
            [JsonProperty("id", Required = Required.Always)]
            public uint Id;
        }
#pragma warning restore 0649

        private static class WebApi
		{
            private const int ApiTimeoutSeconds = 3;

		    internal static HttpClient Client { get; } = new HttpClient
			{
			    BaseAddress = new Uri("https://api.huobi.com/"),  // apiv3
                Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds)
			};

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
			const string queryStr = "staticmarket/ticker_btc_json.js";
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
			var queryStr = string.Format(CultureInfo.InvariantCulture, "staticmarket/depth_btc_{0}.js", limit);
			var json = WebApi.Query(queryStr);
			var book = JsonConvert.DeserializeObject<OrderBook>(json);
			return book;
		}

        /// <summary>
        /// Unique ctor sets access key and secret key, which cannot be changed later.
        /// </summary>
        /// <param name="accessKey">Your Access Key</param>
        /// <param name="secretKey">Your Secret Key</param>
        public HuobiAPI(string accessKey, string secretKey)
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
	        var method = isSell ? "sell" : "buy";
            var args = new NameValueCollection
	        {
		        {"coin_type", ((int)CoinType.Btc).ToString(CultureInfo.InvariantCulture)},
				{"price", price.ToString(CultureInfo.InvariantCulture)},
				{"amount", amount.ToString(CultureInfo.InvariantCulture)}
	        };

			var response = DoMethod2(method, args);
	        var orderResult = JsonConvert.DeserializeObject<HuobiOrderResult>(response);
            if(orderResult.Result != "success")
                throw new HuobiException(method, "Failed to create order: " + method);

            if (orderResult.Id > int.MaxValue)
                throw new HuobiException(method, "Order value too large for int: " + orderResult.Id);

            return (int)orderResult.Id;
		}

		public IEnumerable<Order> GetActiveOrders()
		{
			var args = new NameValueCollection
	        {
                {"coin_type", ((int)CoinType.Btc).ToString(CultureInfo.InvariantCulture)},
	        };

			var response = DoMethod2("get_orders", args);
		    var orders = JsonConvert.DeserializeObject<List<Order>>(response);

//			var jsonObject = CheckResult(response);
//			var orders = jsonObject["orders"].ToObject<List<Order>>();

			return orders;
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
                {"coin_type", ((int)CoinType.Btc).ToString(CultureInfo.InvariantCulture)},
                {"id", orderId.ToString(CultureInfo.InvariantCulture)}
	        };

	        var response = DoMethod2("cancel_order", args);
			var jsonObject = JObject.Parse(response);
			var success = jsonObject.Value<bool>("result");

	        return success;
        }

#pragma warning disable 0649
        // ReSharper disable once ClassNeverInstantiated.Local
        private class HuobiError
        {
            [JsonProperty("code", Required = Required.Always)]
            public int Code { get; private set; }
            [JsonProperty("message", Required = Required.Always)]
            public string Message { get; private set; }
        }
#pragma warning restore 0649

        /// <summary>
        /// Get stored account information and user balance.
        /// </summary>
        /// <returns>objects profile, balance and frozen in JSON</returns>
        public AccountInfo GetAccountInfo()
        {
			var response = DoMethod2("get_account_info", null);
            var userInfo = JsonConvert.DeserializeObject<AccountInfo>(response);
	        return userInfo;
        }

		private string DoMethod2(string method, NameValueCollection jParams)
		{
            // add some more args for authentication
            var seconds = UnixTime.GetFromDateTime(DateTime.UtcNow);

            var args = new NameValueDictionary
		    {
                {"created", seconds.ToString(CultureInfo.InvariantCulture)},
                {"access_key", m_accessKey},
                {"method", method},
                {"secret_key", m_secretKey}
            };

		    if (jParams != null)
			{
				foreach (var key in jParams.AllKeys)
				{
					args.Add(key, jParams[key]);
				}
			}

            var argsSortedByKey = args.OrderBy(kvp => kvp.Key).ToList();

            var sign = GetSignature(argsSortedByKey);
            argsSortedByKey.Add( new KeyValuePair<string, string>("sign", sign));

			var httpContent = new FormUrlEncodedContent(argsSortedByKey);
			var response = WebApi.Client.PostAsync("apiv3/" + method, httpContent).Result;
			var resultString = response.Content.ReadAsStringAsync().Result;

            if (resultString.Contains("code"))
            {
                var error = JsonConvert.DeserializeObject<HuobiError>(resultString);
                throw new HuobiException(method, "Request failed with code: " + error.Code);
            }

            return resultString;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        private static string GetSignature(IEnumerable<KeyValuePair<string, string>> args)
	    {
	        var input = string.Join("&", args.Select(arg => arg.Key + "=" + arg.Value));
            var asciiBytes = Encoding.ASCII.GetBytes(input);
	        using (var md5 = MD5.Create())
	        {
	            var hashedBytes = md5.ComputeHash(asciiBytes);
	            var hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
	            return hashedString;
	        }
	    }
    }
}

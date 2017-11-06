using System;
using System.Globalization;
using System.Net;
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


namespace BTCChina
{
    public sealed class BTCChinaAPI
    {
		private static class WebApi
		{
			private static readonly HttpClient st_client = new HttpClient{ Timeout = TimeSpan.FromSeconds(4) };

			static HttpClient Client { get { return st_client; } }

			public static string Query(string url)
			{
				var resultString = Client.GetStringAsync(url).Result;
				return resultString;
			}
		}

        private readonly string accessKey;
	    private readonly string secretKey;
	    //constants
        private const int BTCChinaConnectionLimit = 1;

		private const string apiHost = "api.btcc.com";
		private const string url = "https://" + apiHost + "/api_trade_v1.php";
        //for cheap access to jParams
        private const string pTonce = "tonce";
        private const string pAccessKey = "accesskey";
        private const string pRequestMethod = "requestmethod";
        private const string pId = "id";
        private const string pMethod = "method";
        private const string pParams = "params";

        private static readonly DateTime genesis = new DateTime(1970, 1, 1);
        // private static readonly Object methodLock = new Object();//protect DoMethod()
        private static readonly Random jsonRequestID = new Random();
		private readonly object m_tonceLock = new object();

        //attributes-like enums for static stringcollection eunmerate
        public enum MarketType { BTCCNY = 0, LTCCNY, LTCBTC, ALL };
        public enum CurrencyType { BTC = 0, LTC };
        public enum TransactionType { all = 0, fundbtc, withdrawbtc, fundmoney, withdrawmoney, refundmoney, buybtc, sellbtc, buyltc, sellltc, tradefee, rebate };


		public static Ticker GetTicker()
		{
			const string queryStr = "https://" + apiHost + "/data/ticker?market=btccny";
			var response = WebApi.Query(queryStr);
			var jobj = JObject.Parse( response );
			return Ticker.ReadFromJObject(jobj["ticker"]);
		}

		public static List<Trade> GetTrades()
		{
			const string queryStr = "https://" + apiHost + "/data/trades";
			var json = WebApi.Query(queryStr);
			var list = JsonConvert.DeserializeObject<List<Trade>>(json);
			return list;
		}

		public static List<TradeHistory> GetTradeHistory(int limit)
		{
			var queryStr = string.Format(CultureInfo.InvariantCulture, "https://" + apiHost + "/data/historydata?limit={0}", limit);
			var json = WebApi.Query(queryStr);
			var list = JsonConvert.DeserializeObject<List<TradeHistory>>(json);
			return list;
		}

	    public static OrderBook GetOrderBook()
	    {
		    return GetOrderBook(20);
	    }

		public static OrderBook GetOrderBook(int limit)
		{
			var queryStr = string.Format(CultureInfo.InvariantCulture, "https://" + apiHost + "/data/orderbook?limit={0}", limit);
			var json = WebApi.Query(queryStr);
			var book = JsonConvert.DeserializeObject<OrderBook>(json);
			return book;
		}

        /// <summary>
        /// Unique ctor sets access key and secret key, which cannot be changed later.
        /// </summary>
        /// <param name="access_key">Your Access Key</param>
        /// <param name="secret_key">Your Secret Key</param>
        public BTCChinaAPI(string access_key, string secret_key)
        {
            accessKey = access_key;
            secretKey = secret_key;
            // HttpWebRequest setups
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; //for https
            ServicePointManager.DefaultConnectionLimit = BTCChinaConnectionLimit; //one concurrent connection is allowed for this server atm.
            //ServicePointManager.UseNagleAlgorithm = false;
        }

        /// <summary>
        /// Place a buy/sell order.
        /// </summary>
        /// <param name="price">The price in quote currency to buy 1 base currency. Negative value to buy/sell at market price</param>
        /// <param name="amount">The amount of LTC/BTC to buy/sell. Negative value to sell, while positive value to buy</param>
        /// <param name="market">Default is "BTCCNY". [ BTCCNY | LTCCNY | LTCBTC ]</param>
        /// <returns>Order id.</returns>
        public int PlaceOrder(double price, double amount, MarketType market)
        {
            string regPrice = "", regAmount = "", method = "", mParams = "";
            switch (market)
            {
                case MarketType.BTCCNY:
					regPrice = price.ToString("F2", CultureInfo.InvariantCulture);
					regAmount = amount.ToString("F4", CultureInfo.InvariantCulture);
                    break;
                case MarketType.LTCCNY:
					regPrice = price.ToString("F2", CultureInfo.InvariantCulture);
					regAmount = amount.ToString("F3", CultureInfo.InvariantCulture);
                    break;
                case MarketType.LTCBTC:
					regPrice = price.ToString("F4", CultureInfo.InvariantCulture);
					regAmount = amount.ToString("F3", CultureInfo.InvariantCulture);
                    break;
                default://"ALL" is not supported
                    throw new BTCChinaException("PlaceOrder", "N/A", "Market not supported.");
            }
            if (regPrice.StartsWith("-", StringComparison.Ordinal))
                regPrice = "null";
			if (regAmount.StartsWith("-", StringComparison.Ordinal))
            {
                regAmount = regAmount.TrimStart('-');
                method = "sellOrder2";
            }
            else
            {
                method = "buyOrder2";
            }

//          mParams = regPrice + "," + regAmount;
            mParams = "\"" + regPrice + "\",\"" + regAmount + "\"";
            //not default market
            if (market != MarketType.BTCCNY)
                mParams += ",\"" + System.Enum.GetName(typeof(MarketType), market) + "\"";

            var response = DoMethod(BuildParams(method, mParams));
	        var jsonObject = JObject.Parse(response);
	        var orderId = jsonObject.Value<int>("result");

	        return orderId;
        }

        /// <summary>
        /// Cancel an active order if the status is 'open'.
        /// </summary>
        /// <param name="orderID">The order id to cancel.</param>
        /// <param name="market">Default is "BTCCNY". [ BTCCNY | LTCCNY | LTCBTC ]</param>
        /// <returns>true or false depending on the result of cancellation.</returns>
        public bool cancelOrder(int orderID, MarketType market=MarketType.BTCCNY)
        {
            const string method = "cancelOrder";
            string mParams = orderID.ToString(CultureInfo.InvariantCulture);
            //all is not supported
            if (market == MarketType.ALL)
                throw new BTCChinaException(method, "N/A", "Market:ALL is not supported.");
            //not default market
            if (market != MarketType.BTCCNY)
                mParams += ",\"" + System.Enum.GetName(typeof(MarketType), market) + "\"";

            var response = DoMethod(BuildParams(method, mParams));
			var jsonObject = JObject.Parse(response);
			var success = jsonObject.Value<bool>("result");

	        return success;
        }


// ReSharper disable once ClassNeverInstantiated.Local
		private class GetAccountInfoResult
		{
			[JsonProperty("result", Required = Required.Always)]
			public AccountInfo Result { get; private set; }
		}

        /// <summary>
        /// Get stored account information and user balance.
        /// </summary>
        /// <returns>objects profile, balance and frozen in JSON</returns>
        public AccountInfo getAccountInfo()
        {
            const string method = "getAccountInfo";
            var response = DoMethod(BuildParams(method, ""));
			var obj = JsonConvert.DeserializeObject<GetAccountInfoResult>(response);
	        return obj.Result;
        }

        /// <summary>
        /// Get all user deposits.
        /// </summary>
        /// <param name="currency">[ BTC | LTC ]</param>
        /// <param name="pendingonly">Default is 'true'. Only open(pending) deposits are returned.</param>
        /// <returns>JSON-string of deposit or withdrawal objects</returns>
        public string getDeposits(CurrencyType currency, bool pendingonly = true)
        {
            const string method = "getDeposits";
            string mParams = "\"" + System.Enum.GetName(typeof(CurrencyType), currency) + "\"";
            if (!pendingonly)
                mParams += "," + pendingonly.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            return DoMethod(BuildParams(method, mParams));
        }

		/// <summary>
        /// Get all user withdrawals.
        /// </summary>
        /// <param name="currency">[ BTC | LTC ]</param>
        /// <param name="pendingonly">Default is 'true'. Only open(pending) deposits are returned.</param>
        /// <returns>JSON-string of deposit or withdrawal objects</returns>
        public string getWithdrawals(CurrencyType currency, bool pendingonly = true)
        {
            const string method = "getWithdrawals";
            string mParams = "\"" + System.Enum.GetName(typeof(CurrencyType), currency) + "\"";
            if (!pendingonly)
				mParams += "," + pendingonly.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            return DoMethod(BuildParams(method, mParams));
        }

        /// <summary>
        /// Get the complete market depth. 
        /// </summary>
        /// <param name="limit">Number of orders returned. Default is 10 per side</param>
        /// <param name="markets">Default to “BTCCNY”. [ BTCCNY | LTCCNY | LTCBTC | ALL]</param>
        /// <returns>All open bid and ask orders.</returns>
        public string getMarketDepth(uint limit = 10, MarketType markets = MarketType.BTCCNY)
        {
            const string method = "getMarketDepth2";
            string mParams = "";
			if (limit != 10) mParams = limit.ToString(CultureInfo.InvariantCulture);
            if (markets != MarketType.BTCCNY)
                mParams += ",\"" + System.Enum.GetName(typeof(MarketType), markets) + "\"";
            return DoMethod(BuildParams(method, mParams));
        }

        /// <summary>
        /// Get withdrawal status.
        /// </summary>
        /// <param name="withdrawalID">The withdrawal id.</param>
        /// <param name="currency">Default is “BTC”. Can be [ BTC | LTC ]</param>
        /// <returns>JSON-string of withdrawal object</returns>
        public string getWithdrawal(int withdrawalID, CurrencyType currency = CurrencyType.BTC)
        {
            const string method = "getWithdrawal";
			string mParams = withdrawalID.ToString(CultureInfo.InvariantCulture);
            if (currency != CurrencyType.BTC)
                mParams += ",\"" + System.Enum.GetName(typeof(CurrencyType), currency) + "\"";//should be "LTC" but for further implmentations
            return DoMethod(BuildParams(method, mParams));
        }

        /// <summary>
        /// Make a withdrawal request. BTC withdrawals will pick last used withdrawal address from user profile.
        /// </summary>
        /// <param name="currency">Currency short code, [BTC | LTC ].</param>
        /// <param name="amount">Amount to withdraw.</param>
        /// <returns>Returns the withdrawal id</returns>
        public string requestWithdrawal(CurrencyType currency, double amount)
        {
            if (amount <= 0)
                throw new BTCChinaException("requestWithdrawal", "N/A", "withdrawal amount cannot be negative nor zero");
            const string method = "requestWithdrawal";
            string mParams = "\"" + System.Enum.GetName(typeof(CurrencyType), currency) + "\"," + amount.ToString("F3", CultureInfo.InvariantCulture);
            return DoMethod(BuildParams(method, mParams));
        }

        /// <summary>
        /// Get order status.
        /// </summary>
        /// <param name="orderID">The order id.</param>
        /// <param name="markets">Default to “BTCCNY”. [ BTCCNY | LTCCNY | LTCBTC ]</param>
        /// <returns>JSON-string of order object.</returns>
        public string getOrder(uint orderID, MarketType markets = MarketType.BTCCNY)
        {
            if (markets == MarketType.ALL)
                throw new BTCChinaException("getOrder", "N/A", "Market: ALL is not supported.");
            else
            {
                const string method = "getOrder";
				string mParams = orderID.ToString(CultureInfo.InvariantCulture);
                if (markets != MarketType.BTCCNY)
                    mParams += ",\"" + System.Enum.GetName(typeof(MarketType), markets) + "\"";
                return DoMethod(BuildParams(method, mParams));
            }
        }

        /// <summary>
        /// Get all order status.
        /// </summary>
        /// <param name="openonly">Default is 'true'. Only open orders are returned.</param>
        /// <param name="markets">Default to “BTCCNY”. [ BTCCNY | LTCCNY | LTCBTC | ALL]</param>
        /// <param name="limit">Limit the number of transactions, default value is 1000.</param>
        /// <param name="offset">Start index used for pagination, default value is 0.</param>
        /// <param name="withdetails">Return the trade details or not for this order. Can be set to true, false. Default to false, no detail will be returned.</param>
        /// <returns>JSON-string of order objects</returns>
        public IEnumerable<Order> getOrders(bool openonly = true, MarketType markets = MarketType.BTCCNY, uint limit = 1000, uint offset = 0, bool withdetails = false)
        {
            //due to the complexity of parameters, all default values are explicitly set.
            const string method = "getOrders";
			string mParams = openonly.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() +
                ",\"" + System.Enum.GetName(typeof(MarketType), markets) + "\"," +
				limit.ToString(CultureInfo.InvariantCulture) + "," +
				offset.ToString(CultureInfo.InvariantCulture) + "," +
				withdetails.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            var response = DoMethod(BuildParams(method, mParams));

			var jobj = JObject.Parse(response);
			if (jobj["result"] != null)
			{
				var orders = jobj["result"]["order"];
				foreach (var orderItem in orders)
				{
					var order = Order.ReadFromJObject(orderItem);
					yield return order;
				}
			};
        }

        /// <summary>
        /// Get transactions log.
        /// </summary>
        /// <param name="transaction">Fetch transactions by type. Default is 'all'. Available types are defined in TransactionType enum.</param>
        /// <param name="limit">Limit the number of transactions, default value is 10.</param>
        /// <param name="offset">Start index used for pagination, default value is 0</param>
        /// <returns>JSON-string of transaction object</returns>
        public string getTransactions(TransactionType transaction = TransactionType.all, uint limit = 10, uint offset = 0)
        {
            //likewise, set all parameters
            const string method = "getTransactions";
            string mParams = "\"" + System.Enum.GetName(typeof(TransactionType), transaction) + "\","
				+ limit.ToString(CultureInfo.InvariantCulture) + ","
				+ offset.ToString(CultureInfo.InvariantCulture);
            return DoMethod(BuildParams(method, mParams));
        }

	    private string DoMethod(NameValueCollection jParams)
	    {
		    const int RequestTimeoutMilliseconds = 2*1000; // 2 sec 

		    string tempResult = "";

		    try
		    {
			    lock (m_tonceLock)
			    {
				    //get tonce
				    TimeSpan timeSpan = DateTime.UtcNow - genesis;
				    long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds*1000);
				    jParams[pTonce] = Convert.ToString(milliSeconds, CultureInfo.InvariantCulture);
				    //mock json request id
				    jParams[pId] = jsonRequestID.Next().ToString(CultureInfo.InvariantCulture);
				    //build http head
				    string paramsHash = GetHMACSHA1Hash(jParams);
				    string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
				    string postData = "{\"method\": \"" + jParams[pMethod] + "\", \"params\": [" + jParams[pParams] + "], \"id\": " +
				                      jParams[pId] + "}";

				    //get webrequest,respawn new object per call for multiple connections
				    var webRequest = (HttpWebRequest) WebRequest.Create(url);
				    webRequest.Timeout = RequestTimeoutMilliseconds;

				    var bytes = Encoding.ASCII.GetBytes(postData);

				    webRequest.Method = jParams[pRequestMethod];
				    webRequest.ContentType = "application/json-rpc";
				    webRequest.ContentLength = bytes.Length;
				    webRequest.Headers["Authorization"] = "Basic " + base64String;
				    webRequest.Headers["Json-Rpc-Tonce"] = jParams[pTonce];

				    // Send the json authentication post request
				    using (var dataStream = webRequest.GetRequestStream())
				    {
					    dataStream.Write(bytes, 0, bytes.Length);
				    }

				    // Get authentication response
				    using (var response = webRequest.GetResponse())
				    {
					    using (var stream = response.GetResponseStream())
					    {
// ReSharper disable once AssignNullToNotNullAttribute
						    using (var reader = new StreamReader(stream))
						    {
							    tempResult = reader.ReadToEnd();
						    }
					    }
				    }
			    }
		    }
		    catch (WebException ex)
		    {
			    throw new BTCChinaException(jParams[pMethod], jParams[pId], ex.Message, ex);
		    }

		    //there are two kinds of API response, result or error.
		    if (tempResult.IndexOf("result", StringComparison.Ordinal) < 0)
		    {
			    throw new BTCChinaException(jParams[pMethod], jParams[pId], "API error:\n" + tempResult);
		    }

		    //compare response id with request id and remove it from result
		    try
		    {
			    int cutoff = tempResult.LastIndexOf(':') + 2;//"id":"1"} so (last index of ':')+2=length of cutoff=start of id-string
			    string idString = tempResult.Substring(cutoff, tempResult.Length - cutoff - 2);//2=last "}
			    if (idString != jParams[pId])
			    {
				    throw new BTCChinaException(jParams[pMethod], jParams[pId], "JSON-request id is not equal with JSON-response id.");
			    }
			    else
			    {
				    //remove json request id from response json string
				    int fromComma = tempResult.LastIndexOf(',');
				    int toLastBrace = tempResult.Length - 1;
				    tempResult = tempResult.Remove(fromComma, toLastBrace - fromComma);
			    }
		    }
		    catch (ArgumentOutOfRangeException ex)
		    {
			    throw new BTCChinaException(jParams[pMethod], jParams[pId], "Argument out of range in parsing JSON response id:" + ex.Message, ex);
		    }

		    return tempResult;
	    }

// ReSharper disable once InconsistentNaming
        private string GetHMACSHA1Hash(NameValueCollection parameters)
        {
            var keyValues = new List<string>();
            foreach (string key in parameters)
            {
                keyValues.Add(key + "=" + parameters[key]);
            }
            string input = String.Join("&", keyValues.ToArray());
            //signature string for hash is NOT JSON-compatible
            //watch out for API changes on the website.
            input = input.Replace("\"", "");
            input = input.Replace("true", "1");
            input = input.Replace("false", "");
            input = input.Replace("null", "");

            //Console.WriteLine(input);

	        using (var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(secretKey)))
	        {
		        byte[] hashData;
		        using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(input)))
		        {
			        hashData = hmacsha1.ComputeHash(stream);
		        }

		        // Format as hexadecimal string.
		        var hashBuilder = new StringBuilder();
		        foreach (byte data in hashData)
		        {
			        hashBuilder.Append(data.ToString("x2", CultureInfo.InvariantCulture));
		        }
		        return hashBuilder.ToString();
	        }
        }

        /// <summary>
        /// build namevaluecollection set for domethod()
        /// </summary>
        /// <param name="method"></param>
        /// <param name="mParams"></param>
        /// <returns></returns>
        private NameValueCollection BuildParams(string method, string mParams)
        {
            return new NameValueCollection
            { 
				{ pTonce, "" },
				{ pAccessKey, accessKey },
				{ pRequestMethod, "post" },//post is supported
				{ pId, "" },
				{ pMethod, method },
				{ pParams, mParams },
			};
        }
    }
}

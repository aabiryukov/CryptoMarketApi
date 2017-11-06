/*
 * Base for making api class for btc-e.com
 */

using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Bitstamp
{
    public class BitstampApi
    {
		private static class WebApi
		{
			private static readonly HttpClient st_client = new HttpClient();

			static WebApi()
			{
				st_client.Timeout = TimeSpan.FromSeconds(2);
			}

			public static HttpClient Client { get { return st_client; } }

			public static string Query(string url)
			{
				var resultString = Client.GetStringAsync(url).Result;
				return resultString;
			}
		}

		private class NameValueDictionary : Dictionary<string, string> { }

		private readonly string m_clientId;
		private readonly string m_key;
		private readonly string m_secret;
		private readonly object m_nonceLock = new object();
		private int m_nonce;

//	    readonly HMACSHA512 m_hashMaker;
 //       UInt32 m_nonce;
        public BitstampApi(string key, string secret, string clientId)
        {
            m_key = key;
	        m_secret = secret;
	        m_clientId = clientId;
 //           m_hashMaker = new HMACSHA512(Encoding.ASCII.GetBytes(secret));
//            m_nonce = UnixTime.Now;

			m_nonce = (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

	    private static void CheckBitstampError(string json)
	    {
			if(!json.Contains("error"))
				return;

			var jsonObject = JObject.Parse(json);
			var errorText = jsonObject["error"];
			
			if (errorText != null)
				throw new BitstampException("Bitstamp error: " + errorText);
	    }

		public static T DeserializeBitstampObject<T>(string json)
		{
			CheckBitstampError(json);
			var obj = JsonConvert.DeserializeObject<T>(json);
			return obj;
		}

		public Balance GetBalance()
        {
			var resultStr = Query("balance");

			return DeserializeBitstampObject<Balance>(resultStr);
        }

		public List<Order> GetOpenOrders()
	    {
			var resultStr = Query("open_orders");

			if (resultStr == "[]")
				return new List<Order>();

			var orders = DeserializeBitstampObject<List<Order>>(resultStr);

			return orders;
		}

		public TradeAnswer Buy(decimal price, decimal amount)
		{
			var args = new NameValueDictionary
            {
                { "amount", DecimalToString(amount) },
                { "price", DecimalToString(price) },
            };

			var json = Query("buy", args);
			var answer = DeserializeBitstampObject<TradeAnswer>(json);
			return answer;
		}

		public TradeAnswer Sell(decimal price, decimal amount)
		{
			var args = new NameValueDictionary
            {
                { "amount", DecimalToString(amount) },
                { "price", DecimalToString(price) },
            };

			var json = Query("sell", args);
			var answer = DeserializeBitstampObject<TradeAnswer>(json);
			return answer;
		}
		
        public bool CancelOrder(int orderId)
        {
            var args = new NameValueDictionary
            {
                { "id", orderId.ToString(CultureInfo.InvariantCulture) }
            };
			var json = Query("cancel_order", args);
			var result = DeserializeBitstampObject<bool>(json);

	        return result;
        }

// ReSharper disable once InconsistentNaming
		private static byte[] SignHMACSHA256(String key, byte[] data)
		{
			using (var hashMaker = new HMACSHA256(Encoding.ASCII.GetBytes(key)))
			{
				return hashMaker.ComputeHash(data);
			}
		}

		private static byte[] StrinToByteArray(string str)
		{
			return Encoding.ASCII.GetBytes(str);
		}

		private string GetSignature(int nonce)
		{
			var msg = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
				nonce,
				m_clientId,
				m_key);

			return ByteArrayToString(SignHMACSHA256(
				m_secret, StrinToByteArray(msg))).ToUpper(CultureInfo.InvariantCulture);
		}

		private string Query(string operation, NameValueDictionary args = null)
        {
			if(args == null)
				args = new NameValueDictionary();

			lock (m_nonceLock)
			{
				var nonce = GetNonce();
				var signature = GetSignature(nonce);

				args.Add("key", m_key);
				args.Add("signature", signature);
				args.Add("nonce", nonce.ToString(CultureInfo.InvariantCulture));

				var path = new Uri("https://www.bitstamp.net/api/" + operation + "/");

				var httpContent = new FormUrlEncodedContent(args);
				var response = WebApi.Client.PostAsync(path, httpContent).Result;
				var resultString = response.Content.ReadAsStringAsync().Result;
				return resultString;
			}
        }

        static string ByteArrayToString(byte[] ba)
        {
			var hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

	    private int GetNonce()
        {
			return m_nonce++;
        }

        static string DecimalToString(decimal d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        public static OrderBook GetOrderBook()
        {
			const string queryStr = "https://www.bitstamp.net/api/order_book/";
	        var json = WebApi.Query(queryStr);
	        var book = DeserializeBitstampObject<OrderBook>(json);
	        return book;
        }

        public static Ticker GetTicker()
        {
			const string queryStr = "https://www.bitstamp.net/api/ticker/";
			var response = WebApi.Query(queryStr);
	        var jobj = JObject.Parse(response);
			return Ticker.ReadFromJObject(jobj);
        }

        public static List<Transaction> GetTransactions()
        {
			const string queryStr = "https://www.bitstamp.net/api/transactions/";
			var json = WebApi.Query(queryStr);
	        var list = JsonConvert.DeserializeObject<List<Transaction>>(json);
	        return list;
        }

    }
}

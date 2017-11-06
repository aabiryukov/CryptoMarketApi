/*
 * Base for making api class for btc-e.com
 * DmT
 * 2012
 */

using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BtcE
{
    internal static class WebApi
	{
        internal const string RootUrl = "https://btc-e.nz";
        private static readonly HttpClient st_client = new HttpClient();

		static WebApi()
		{
			st_client.Timeout = TimeSpan.FromSeconds(2);
		}

		internal static HttpClient Client { get { return st_client; } }

		internal static string Query(string url)
		{
			var resultString = Client.GetStringAsync(url).Result;
			return resultString;
		}
	}

    public class BtceApi
    {
        private class NameValueDictionary : Dictionary<string, string> { }
		
		private readonly string m_key;
		private readonly HMACSHA512 m_hashMaker;
		private readonly object m_nonceLock = new object();
		private UInt32 m_nonce;

	    public BtceApi(string key, string secret)
        {
            m_key = key;
            m_hashMaker = new HMACSHA512(Encoding.ASCII.GetBytes(secret));
            m_nonce = UnixTime.Now;
        }

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable once ClassNeverInstantiated.Local
		private class ReturnData<T>
		{
			[JsonProperty("success", Required = Required.Always)]
			public int Success { get; set; }
			[JsonProperty("error", Required = Required.Default)]
			public string ErrorText { get; set; }
			[JsonProperty("return", Required = Required.Default)]
			public T Object { get; set; }
		}
// ReSharper restore UnusedAutoPropertyAccessor.Local

		private static T DeserializeBtceObject<T>(string json)
			where T: class 
		{
//			CheckError(json);
			var returnData = JsonConvert.DeserializeObject<ReturnData<T>>(json);

			if (returnData.Success != 1)
			{
				throw new BtcApiException(string.Format(CultureInfo.CurrentCulture, "BTC-E error [succ={0}]: {1}", returnData.Success, returnData.ErrorText));
			}

			if (returnData.Object == null)
			{
				throw new BtcApiException(string.Format(CultureInfo.CurrentCulture, "BTC-E error [succ={0}, type={1}]: Returned object is null", returnData.Success, typeof(T)));
			}

			return returnData.Object;
		}

        public UserInfo GetInfo()
        {
			var json = Query("getInfo");
			var userInfo = DeserializeBtceObject<UserInfo>(json);
	        return userInfo;
        }

        public TransHistory GetTransHistory(
            int? from = null,
            int? count = null,
            int? fromId = null,
            int? endId = null,
            bool? orderAsc = null,
            DateTime? since = null,
            DateTime? end = null
            )
        {
            var args = new NameValueDictionary();

            if (from != null) args.Add("from", from.Value.ToString(CultureInfo.InvariantCulture));
			if (count != null) args.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
			if (fromId != null) args.Add("from_id", fromId.Value.ToString(CultureInfo.InvariantCulture));
			if (endId != null) args.Add("end_id", endId.Value.ToString(CultureInfo.InvariantCulture));
            if (orderAsc != null) args.Add("order", orderAsc.Value ? "ASC" : "DESC");
			if (since != null) args.Add("since", UnixTime.GetFromDateTime(since.Value).ToString(CultureInfo.InvariantCulture));
			if (end != null) args.Add("end", UnixTime.GetFromDateTime(end.Value).ToString(CultureInfo.InvariantCulture));
            
			var json = Query("TransHistory", args);
	        var result = DeserializeBtceObject<TransHistory>(json);
	        return result;
        }

        public TradeHistory GetTradeHistory(
            int? from = null,
            int? count = null,
            int? fromId = null,
            int? endId = null,
            bool? orderAsc = null,
            DateTime? since = null,
            DateTime? end = null
            )
        {
			var args = new NameValueDictionary();

			if (from != null) args.Add("from", from.Value.ToString(CultureInfo.InvariantCulture));
			if (count != null) args.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
			if (fromId != null) args.Add("from_id", fromId.Value.ToString(CultureInfo.InvariantCulture));
			if (endId != null) args.Add("end_id", endId.Value.ToString(CultureInfo.InvariantCulture));
            if (orderAsc != null) args.Add("order", orderAsc.Value ? "ASC" : "DESC");
			if (since != null) args.Add("since", UnixTime.GetFromDateTime(since.Value).ToString(CultureInfo.InvariantCulture));
			if (end != null) args.Add("end", UnixTime.GetFromDateTime(end.Value).ToString(CultureInfo.InvariantCulture));

			var json = Query("TradeHistory", args);
			var result = DeserializeBtceObject<TradeHistory>(json);
			return result;
        }

	    public OrderList ActiveOrders(BtcePair? pair = null)
	    {
			var args = new NameValueDictionary();

			if (pair != null) 
				args.Add("pair", BtcePairHelper.ToString(pair.Value));

			var json = Query("ActiveOrders", args);

		    if (json.Contains("\"no orders\""))
		    {
			    return new OrderList();
		    }

			var result = DeserializeBtceObject<OrderList>(json);
			return result;
		}

        public TradeAnswer Trade(BtcePair pair, TradeType type, decimal rate, decimal amount)
        {
			var args = new NameValueDictionary
            {
                { "pair", BtcePairHelper.ToString(pair) },
                { "type", TradeTypeHelper.ToString(type) },
                { "rate", DecimalToString(rate) },
                { "amount", DecimalToString(amount) }
            };
            var result = JObject.Parse(Query("Trade", args));
            if (result.Value<int>("success") == 0)
				throw new BtcApiException(result.Value<string>("error"));
            return TradeAnswer.ReadFromJObject(result["return"] as JObject);
        }

        public CancelOrderAnswer CancelOrder(int orderId)
        {
			var args = new NameValueDictionary
            {
                { "order_id", orderId.ToString(CultureInfo.InvariantCulture) }
            };
            var result = JObject.Parse(Query("CancelOrder", args));

            if (result.Value<int>("success") == 0)
				throw new BtcApiException(result.Value<string>("error"));

            return CancelOrderAnswer.ReadFromJObject(result["return"] as JObject);
        }

	    private string Query(string methodName)
	    {
			var args = new NameValueDictionary
		    {
			    {"method", methodName},
		    };

			return InternalQuery(args);
	    }

		private string Query(string methodName, NameValueDictionary args)
		{
			args["method"] = methodName;
			return InternalQuery(args);
		}


	    private string InternalQuery(NameValueDictionary args)
	    {
		    lock (m_nonceLock)
		    {
			    args.Add("nonce", GetNonce().ToString(CultureInfo.InvariantCulture));

			    var dataStr = BuildPostData(args);
			    var data = Encoding.ASCII.GetBytes(dataStr);

                using (var httpContent = new FormUrlEncodedContent(args))
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, WebApi.RootUrl + "/tapi"))
                    {
                        request.Headers.Add("Key", m_key);
                        request.Headers.Add("Sign", ByteArrayToString(m_hashMaker.ComputeHash(data)).ToLowerInvariant());
                        request.Content = httpContent;

                        var response = WebApi.Client.SendAsync(request).Result;
                        var resultString = response.Content.ReadAsStringAsync().Result;
                        return resultString;
                    }
                }
		    }
	    }

	    static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

		static string BuildPostData(NameValueDictionary d)
        {
            var s = new StringBuilder();
            foreach (var key in d.Keys)
            {
	            var value = d[key];
				s.AppendFormat("{0}={1}", key, WebUtility.UrlEncode(value));
                s.Append("&");
            }
            if (s.Length > 0) s.Remove(s.Length - 1, 1);
            return s.ToString();
        }

        UInt32 GetNonce()
        {
	        return m_nonce++;
        }
        static string DecimalToString(decimal d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }
        public static Depth GetDepth(BtcePair pair)
        {
			string queryStr = string.Format(CultureInfo.InvariantCulture, WebApi.RootUrl + "/api/2/{0}/depth", BtcePairHelper.ToString(pair));
	        var json = WebApi.Query(queryStr);
			var result = JsonConvert.DeserializeObject<Depth>(json);
			return result;

//            return Depth.ReadFromJObject(JObject.Parse(json));
        }
        public static Ticker GetTicker(BtcePair pair)
        {
			string queryStr = string.Format(CultureInfo.InvariantCulture, WebApi.RootUrl + "/api/2/{0}/ticker", BtcePairHelper.ToString(pair));
	        var json = WebApi.Query(queryStr);
			return Ticker.ReadFromJObject(JObject.Parse(json)["ticker"] as JObject);
        }
        public static List<TradeInfo> GetTrades(BtcePair pair)
        {
			string queryStr = string.Format(CultureInfo.InvariantCulture, WebApi.RootUrl + "/api/2/{0}/trades", BtcePairHelper.ToString(pair));
	        var json = WebApi.Query(queryStr);
			var result = JsonConvert.DeserializeObject<List<TradeInfo>>(json);
	        return result;
//            return JArray.Parse(json).OfType<JObject>().Select(TradeInfo.ReadFromJObject).ToList();
        }
        public static decimal GetFee(BtcePair pair)
        {
            string queryStr = string.Format(CultureInfo.InvariantCulture, WebApi.RootUrl + "/api/2/{0}/fee", BtcePairHelper.ToString(pair));
	        var json = WebApi.Query(queryStr);
            return JObject.Parse(json).Value<decimal>("trade");
        }
    }

    public static class BtceApiV3
    {
        private static string MakePairListString(IEnumerable<BtcePair> pairlist)
        {
            return string.Join("-", pairlist.Select(BtcePairHelper.ToString).ToArray());
        }

        private static string Query(string method, IEnumerable<BtcePair> pairlist, Dictionary<string, string> args = null)
        {
            var pairliststr = MakePairListString(pairlist);
            var sb = new StringBuilder();
            sb.Append(WebApi.RootUrl + "/api/3/");
            sb.Append(method);
            sb.Append("/");
            sb.Append(pairliststr);
            if (args != null && args.Count > 0)
            {
                sb.Append("?");
				var arr = args.Select(x => string.Format(CultureInfo.InvariantCulture, "{0}={1}", WebUtility.UrlEncode(x.Key), WebUtility.UrlEncode(x.Value))).ToArray();
                sb.Append(string.Join("&", arr));
            }
            var queryStr = sb.ToString();
            return WebApi.Query(queryStr);
        }

/*
        private static string QueryIgnoreInvalid(string method, IEnumerable<BtcePair> pairlist, Dictionary<string, string> args = null)
        {
            var newargs = new Dictionary<string,string> { {"ignore_invalid", "1"} };
	        if (args != null)
	        {
		        foreach (var arg in args)
		        {
			        newargs.Add(arg.Key, arg.Value);
		        }
	        }
	        return Query(method, pairlist, newargs);
        }
*/

        private static Dictionary<BtcePair, T> ReadPairDict<T>(JObject o, Func<JContainer, T> valueReader)
        {
            return o.OfType<JProperty>().Select(x => new KeyValuePair<BtcePair, T>(BtcePairHelper.FromString(x.Name), valueReader(x.Value as JContainer))).ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<BtcePair, T> MakeRequest<T>(string method, IEnumerable<BtcePair> pairlist, Func<JContainer, T> valueReader, Dictionary<string, string> args = null)
        {
/*
	        bool ignoreInvalid = true;
	        var queryresult = ignoreInvalid 
				? QueryIgnoreInvalid(method, pairlist, args) 
				: Query(method, pairlist, args);
*/
			var queryresult = Query(method, pairlist, args);
            var resobj = JObject.Parse(queryresult);

            if (resobj["success"] != null && resobj.Value<int>("success") == 0)
				throw new BtcApiException(resobj.Value<string>("error"));

            var r = ReadPairDict(resobj, valueReader);
            return r;
        }

        public static Dictionary<BtcePair, Depth> GetDepth(BtcePair[] pairlist, int limit = 150)
        {
			return MakeRequest(
				"depth", 
				pairlist, 
				x => Depth.ReadFromJObject(x as JObject), 
				new Dictionary<string, string> { { "limit", limit.ToString(CultureInfo.InvariantCulture) } }
				);
        }

        public static Dictionary<BtcePair, Ticker> GetTicker(BtcePair[] pairlist)
        {
            return MakeRequest("ticker", pairlist, x => Ticker.ReadFromJObject(x as JObject));
        }

        public static Dictionary<BtcePair, List<TradeInfoV3>> GetTrades(BtcePair[] pairlist, int limit = 150)
        {
            var limits = new Dictionary<string, string> { { "limit", limit.ToString(CultureInfo.InvariantCulture) } };

            Func<JContainer, List<TradeInfoV3>> tradeInfoListReader = (x => x.OfType<JObject>().Select(TradeInfoV3.ReadFromJObject).ToList());
			return MakeRequest("trades", pairlist, tradeInfoListReader, limits);
        }


    }
}

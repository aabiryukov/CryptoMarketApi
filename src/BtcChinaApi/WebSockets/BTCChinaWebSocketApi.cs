using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using NLog;
using WebSocket4Net;

namespace BTCChina.WebSockets
{
	public class GroupOrderReceivedEventArgs : EventArgs
	{
		public WsGroupOrder GroupOrder { get; private set; }
		public GroupOrderReceivedEventArgs(WsGroupOrder groupOrder)
		{
			GroupOrder = groupOrder;
		}
	}

	public class BtcChinaWebSocketApi: IDisposable
	{
		// ReSharper disable once ClassNeverInstantiated.Local
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		private class WsConfigHelper
		{
			[JsonProperty("sid")]
			public string Sid { get; set; }
			[JsonProperty("upgrades")]
// ReSharper disable once UnusedMember.Local
			public List<string> Upgrades { get; set; }
			[JsonProperty("pingInterval")]
			public int PingInterval { get; set; }
			[JsonProperty("pingTimeout")]
			public int PingTimeout { get; set; }
		}
		// ReSharper restore UnusedAutoPropertyAccessor.Local

		// ReSharper disable InconsistentNaming
		//v1.0 message types
		private enum EngineioMessageType
		{
//			OPEN = 0,//non-ws
//			CLOSE = 1,//non-ws
			/*
			 * Pings server every "pingInterval" and expects response
			 * within "pingTimeout" or closes connection.
			 * 
			 * client sends ping, waiting for server's pong.
			 * socket.io message type is not necessary in ping/pong.
			 * 
			 * client sends pong after receiving server's ping.
			 */
			PING = 2,
			PONG = 3,

			MESSAGE = 4,//TYPE_EVENT in v0.9.x
			UPGRADE = 5, //new in v1.0
//			NOOP = 6
		}
		private enum SocketioMessageType
		{
			CONNECT = 0,//right after engine.io UPGRADE
			DISCONNECT = 1,
			EVENT = 2,
//			ACK = 3,
//			ERROR = 4,
//			BINARY_EVENT = 5,
//			BINARY_ACK = 6
		}
		//every transport between server and client is formatted as: "engine.io Message Type" + "socket.io Message Type" + json-encoded content.
		// ReSharper restore InconsistentNaming

// ReSharper disable once ClassNeverInstantiated.Local
		private class WsGroupOrderMessage
		{
			[JsonProperty("grouporder", Required = Required.Always)]
// ReSharper disable once UnusedAutoPropertyAccessor.Local
			 public WsGroupOrder GroupOrder { get; set; }
		}

// ReSharper disable once InconsistentNaming
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private WebSocket m_webSocket;

		private Timer m_pingIntervalTimer, m_pingTimeoutTimer;
		private bool m_pong;

		public event EventHandler<GroupOrderReceivedEventArgs> GroupOrderReceived;
		public event EventHandler<EventArgs> TimeoutReceived;

		public bool IsClosed
		{
			get { return m_webSocket.State == WebSocketState.Closed; }
		}

		public void Start()
		{
			if(m_webSocket != null)
				return;

			const string httpScheme = "https://";
			const string wsScheme = "wss://";
			const string webSocketUrl = "websocket.btcc.com/socket.io/";

			#region handshake

			string polling;
			using (var wc = new HttpClient())
			{
				polling = wc.GetStringAsync(httpScheme + webSocketUrl + "?transport=polling").Result;
				if (string.IsNullOrEmpty(polling))
				{
					throw new BTCChinaException("BtcChinaWebSocketApi.Start", "", "failed to download config");
				}
			}

			var config = polling.Substring(polling.IndexOf('{'), polling.IndexOf('}') - polling.IndexOf('{') + 1);
			var wsc = JsonConvert.DeserializeObject<WsConfigHelper>(config);

			#endregion handshake

			//set timers
			m_pingTimeoutTimer = new Timer(_ =>
			{
				if (m_pong)
				{
					m_pong = false; //waiting for another ping
				}
				else
				{
					Log.Error("[BtcChina] Ping Timeout!");
					if (TimeoutReceived != null)
					{
						TimeoutReceived(this, new EventArgs());
					}
				}
			}, null, Timeout.Infinite, Timeout.Infinite);

			m_pingIntervalTimer = new Timer(_ =>
			{
				m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}", (int)EngineioMessageType.PING));
				m_pingTimeoutTimer.Change(wsc.PingTimeout, Timeout.Infinite);
				m_pong = false;
			}, null, wsc.PingInterval, wsc.PingInterval);

			//setup websocket connections and events
			m_webSocket = new WebSocket(wsScheme + webSocketUrl + "?transport=websocket&sid=" + wsc.Sid);
			m_webSocket.Opened += btc_Opened;
			m_webSocket.Error += btc_Error;
			m_webSocket.MessageReceived += btc_MessageReceived;
			m_webSocket.DataReceived += btc_DataReceived;
			m_webSocket.Closed += btc_Closed;

			Log.Info("[BtcChina] Opening websockets...");
			m_webSocket.Open();
		}

		public void Stop()
		{
			if (m_webSocket == null)
				return;

			//close the connection.
			m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}{1}", (int)EngineioMessageType.MESSAGE, (int)SocketioMessageType.DISCONNECT));
//			Thread.Sleep(100);

			using (var timerDisposed = new ManualResetEvent(false))
			{
				m_pingIntervalTimer.Dispose(timerDisposed);
				timerDisposed.WaitOne();
				m_pingIntervalTimer = null;
			}

			using (var timerDisposed = new ManualResetEvent(false))
			{
				m_pingTimeoutTimer.Dispose(timerDisposed);
				timerDisposed.WaitOne();
				m_pingTimeoutTimer = null;
			}

			m_webSocket.Close();
			m_webSocket.Dispose();
			m_webSocket = null;
		}
/*
		public static void ThrowMessage(string message)
		{
			Console.WriteLine("BtcChina: " + message);
		}
*/
		private static void btc_Closed(object sender, EventArgs e)
		{
			Log.Info("[BtcChina] Websocket closed.");
		}

		private static void btc_DataReceived(object sender, DataReceivedEventArgs e)
		{
			Log.Info("[BtcChina] data:" + e.Data);
		}

		private void btc_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			int eioMessageType;
			if (int.TryParse(e.Message.Substring(0, 1), out eioMessageType))
			{
				switch ((EngineioMessageType)eioMessageType)
				{
					case EngineioMessageType.PING:
						//replace incoming PING with PONG in incoming message and resend it.
						m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}{1}", (int)EngineioMessageType.PONG, e.Message.Substring(1, e.Message.Length - 1)));
						break;
					case EngineioMessageType.PONG:
						m_pong = true;
						break;

					case EngineioMessageType.MESSAGE:
						int sioMessageType;
						if (int.TryParse(e.Message.Substring(1, 1), out sioMessageType))
						{
							switch ((SocketioMessageType)sioMessageType)
							{
								case SocketioMessageType.CONNECT:
									//Send "42["subscribe",["marketdata_cnybtc","marketdata_cnyltc","marketdata_btcltc"]]"
									m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", (int)EngineioMessageType.MESSAGE,
																	   (int)SocketioMessageType.EVENT,
//																	   "[\"subscribe\",[\"marketdata_cnybtc\",\"marketdata_cnyltc\",\"grouporder_cnybtc\"]]"
																	   "[\"subscribe\",[\"grouporder_cnybtc\"]]"
																	   )
																	   );

									break;

								case SocketioMessageType.EVENT:
									if (e.Message.Substring(4, 5) == "trade")//listen on "trade"
										Log.Info("[BtcChina] TRADE: " + e.Message.Substring(e.Message.IndexOf('{'), e.Message.LastIndexOf('}') - e.Message.IndexOf('{') + 1));
									else
										if (e.Message.Substring(4, 10) == "grouporder")//listen on "trade")
										{
											Log.Info("[BtcChina] grouporder event");

											var json = e.Message.Substring(e.Message.IndexOf('{'), e.Message.LastIndexOf('}') - e.Message.IndexOf('{') + 1);
											var objResponse = JsonConvert.DeserializeObject<WsGroupOrderMessage>(json);
											OnMessageGroupOrder(objResponse.GroupOrder);
										}
										else
										{
											Log.Warn("[BtcChina] Unknown message: " + e.Message.Substring(0, 100));
										}
									break;

								default:
									Log.Error("[BtcChina] error switch socket.io messagetype: " + e.Message);
									break;
							}
						}
						else
						{
							Log.Error("[BtcChina] error parse socket.io messagetype!");
						}
						break;

					default:
						Log.Error("[BtcChina] error switch engine.io messagetype");
						break;
				}
			}
			else
			{
				Log.Error("[BtcChina] error parsing engine.io messagetype!");
			}
		}

		private void OnMessageGroupOrder(WsGroupOrder groupOrder)
		{
			if (GroupOrderReceived != null)
			{
				GroupOrderReceived(this, new GroupOrderReceivedEventArgs(groupOrder));
			}
//			ThrowMessage("GROUPORDER: " + groupOrder.Market);
		}

		private static void btc_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
			Log.Error("[BtcChina] Websocket ERROR: " + e.Exception.Message);
		}

		private void btc_Opened(object sender, EventArgs e)
		{
			//send upgrade message:"52"
			//server responses with message: "40" - message/connect
			Log.Info("[BtcChina] Websocket opened.");
			m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}{1}", (int)EngineioMessageType.UPGRADE, (int)SocketioMessageType.EVENT));
		}

		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
			}
		}
	}
}

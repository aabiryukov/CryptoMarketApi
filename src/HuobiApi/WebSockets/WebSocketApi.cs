using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using WebSocket4Net;

namespace Huobi
{
	public class OrdersDepthReceivedEventArgs : EventArgs
	{
		public OrderBook OrdersDepth { get; private set; }
		public OrdersDepthReceivedEventArgs(OrderBook ordersDepth)
		{
			OrdersDepth = ordersDepth;
		}
	}

	public class WebSocketApi: IDisposable
	{
		private const int PingInterval = 3000;
		private const int PingTimeout = 5000;

// ReSharper disable once InconsistentNaming
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private WebSocket m_webSocket;

		private Timer m_pingIntervalTimer, m_pingTimeoutTimer;
		private bool m_pong;

		public event EventHandler<OrdersDepthReceivedEventArgs> OrdersDepthReceived;
		public event EventHandler<EventArgs> TimeoutReceived;

		public bool IsClosed
		{
			get { return m_webSocket.State == WebSocketState.Closed; }
		}

		public void Start()
		{
			if(m_webSocket != null)
				return;

			const string webSocketUrl = "wss://real.okcoin.com:10440/websocket/okcoinapi";

			//set timers
			m_pingTimeoutTimer = new Timer(_ =>
			{
				if (m_pong)
				{
					m_pong = false; //waiting for another ping
				}
				else
				{
					Log.Error("[Huobi] Ping Timeout!");
					if (TimeoutReceived != null)
					{
						TimeoutReceived(this, new EventArgs());
					}
				}
			}, null, Timeout.Infinite, Timeout.Infinite);

			m_pingIntervalTimer = new Timer(_ =>
			{
				m_webSocket.Send("{'event':'ping'}");
				m_pingTimeoutTimer.Change(PingTimeout, Timeout.Infinite);
				m_pong = false;
			}, null, PingInterval, PingInterval);

			//setup websocket connections and events
			m_webSocket = new WebSocket(webSocketUrl);
			m_webSocket.Opened += btc_Opened;
			m_webSocket.Error += btc_Error;
			m_webSocket.MessageReceived += btc_MessageReceived;
			m_webSocket.DataReceived += btc_DataReceived;
			m_webSocket.Closed += btc_Closed;

			Log.Info("[Huobi] Opening websockets...");
			m_webSocket.Open();
		}

		public void Stop()
		{
			if (m_webSocket == null)
				return;

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
			Console.WriteLine("Huobi: " + message);
		}
*/
		private static void btc_Closed(object sender, EventArgs e)
		{
			Log.Info("[Huobi] Websocket closed.");
		}

		private static void btc_DataReceived(object sender, DataReceivedEventArgs e)
		{
			Log.Info("[Huobi] data:" + e.Data);
		}

		private void btc_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			if (e.Message == "{\"event\":\"pong\"}")
			{
				m_pong = true;
				return;
			}

			try
			{
				var jchannels = JArray.Parse(e.Message);

				foreach (var jchannel in jchannels)
				{
					var channelName = jchannel.Value<string>("channel");
					switch (channelName)
					{
						case "ok_btcusd_depth":
							{
								var ordersDepth = jchannel["data"].ToObject<OrderBook>();
								OnMessageGroupOrder(ordersDepth);
							}
							break;

						default:
							Log.Error("[Huobi] Unknown channel name: " + channelName);
							break;
					}
				}
			}
			catch (JsonException exception)
			{
				Log.Error("[Huobi][Json error] " + exception.Message);
			}
		}

		private void OnMessageGroupOrder(OrderBook ordersDepth)
		{
			if (OrdersDepthReceived != null)
			{
				OrdersDepthReceived(this, new OrdersDepthReceivedEventArgs(ordersDepth));
			}
//			ThrowMessage("GROUPORDER: " + groupOrder.Market);
		}

		private static void btc_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
			Log.Error("[Huobi] Websocket ERROR: " + e.Exception.Message);
		}

		private void btc_Opened(object sender, EventArgs e)
		{
			//send upgrade message:"52"
			//server responses with message: "40" - message/connect
			Log.Info("[Huobi] Websocket opened.");
//			m_webSocket.Send(string.Format(CultureInfo.InvariantCulture, "{0}{1}", (int)EngineioMessageType.UPGRADE, (int)SocketioMessageType.EVENT));

			const string dataMsg = "{'event':'addChannel','channel':'ok_btcusd_depth'}";
			m_webSocket.Send(dataMsg);
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

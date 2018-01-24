using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Logging;
using Bitfinex.Json.SocketObjets;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bitfinex.Errors;
using System.Diagnostics;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using Bitfinex.Json.Objects;

namespace Bitfinex
{
    public class BitfinexSocketApi: BitfinexAbstractApi
    {
        private const string BaseAddress = "wss://api.bitfinex.com/ws/2";
        private const string AuthenticationSucces = "OK";

        private const string PositionsSnapshotEvent = "ps";
        private const string WalletsSnapshotEvent = "ws";
        private const string OrdersSnapshotEvent = "os";
        private const string FundingOffersSnapshotEvent = "fos";
        private const string FundingCreditsSnapshotEvent = "fcs";
        private const string FundingLoansSnapshotEvent = "fls";
        private const string ActiveTradesSnapshotEvent = "ats"; // OK?
        private const string HeartbeatEvent = "hb";

        private static WebSocket socket;
//        private static bool authenticated;

        private List<BitfinexEventRegistration> eventRegistrations = new List<BitfinexEventRegistration>();

        private string nonce => Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 10).ToString(CultureInfo.InvariantCulture);

        private object subscriptionLock = new object();
        private object registrationIdLock = new object();
        private object eventListLock = new object();

        private long _regId;
        private long registrationId
        {
            get
            {
                lock (registrationIdLock)
                {
                    return ++_regId;
                }
            }
        }

        public BitfinexSocketApi()
        {
        }

        public BitfinexSocketApi(string apiKey, string apiSecret)
        {
            SetApiCredentials(apiKey, apiSecret);
        }

        public void Connect()
        {
            socket = new WebSocket(BaseAddress);
//            socket.Log.Level = LogLevel.Info;
            socket.Closed += SocketClosed;
            socket.Error += SocketError;
            socket.Opened += SocketOpened;
            socket.MessageReceived += SocketMessage;

            socket.Open();
        }

        private void SocketClosed(object sender, EventArgs e)
        {
            Log.Write(LogVerbosity.Info, "Socket closed");
        }

        private void SocketError(object sender, ErrorEventArgs args)
        {
            Log.Write(LogVerbosity.Error, $"Socket error: {args.Exception.GetType().Name} - {args.Exception.Message}");
        }

        private void SocketOpened(object sender, EventArgs args)
        {
            Log.Write(LogVerbosity.Info, $"Socket opened");

            if(!string.IsNullOrEmpty(ApiKey) && HasSecretKey)
                Authenticate();
        }

        private void SocketMessage(object sender, MessageReceivedEventArgs args)
        {
            var dataObject = JToken.Parse(args.Message);
            if(dataObject is JObject)
            {
                Log.Write(LogVerbosity.Info, $"Received object message: {dataObject}");
                var evnt = dataObject["event"].ToString();
                if (evnt == "info")
                    HandleInfoEvent(dataObject.ToObject<BitfinexInfo>());
                else if (evnt == "auth")
                    HandleAuthenticationEvent(dataObject.ToObject<BitfinexAuthenticationResponse>());
                else if (evnt == "subscribed")
                    HandleSubscriptionEvent(dataObject.ToObject<BitfinexSubscriptionResponse>());
                else if (evnt == "error")
                    HandleErrorEvent(dataObject.ToObject<BitfinexSocketError>());                
                else
                    HandleUnhandledEvent((JObject)dataObject);                
            }
            else if(dataObject is JArray)
            {
                Log.Write(LogVerbosity.Info, $"Received array message: {dataObject}");
                if(dataObject[1].ToString() == "hb")
                {
                    // Heartbeat, no need to do anything with that
                    return;
                }

                if (dataObject[0].ToString() == "0")
                    HandleAccountEvent(dataObject.ToObject<BitfinexSocketEvent>());
                else
                    HandleChannelEvent((JArray)dataObject);
            }
        }

        private void Authenticate()
        {
            var n = nonce;
            var authentication = new BitfinexAuthentication()
            {
                Event = "auth",
                ApiKey = ApiKey,
                Nonce = n,
                Payload = "AUTH" + n
            };
            authentication.Signature = GetHexHashSignature(authentication.Payload);

            socket.Send(JsonConvert.SerializeObject(authentication));
        }

        private void HandleAuthenticationEvent(BitfinexAuthenticationResponse response)
        {
            if(response.Status == AuthenticationSucces)
            {
//                authenticated = true;
                Log.Write(LogVerbosity.Info, $"Socket authentication successful, authentication id : {response.AuthenticationId}");
            }
            else
            {
                Log.Write(LogVerbosity.Warning, $"Socket authentication failed. Status: {response.Status}, Error code: {response.ErrorCode}, Error message: {response.ErrorMessage}");
            }
        }

        private void HandleInfoEvent(BitfinexInfo info)
        {
            if (info.Version != 0)
                Log.Write(LogVerbosity.Info, $"API protocol version {info.Version}");

            if (info.Code != 0)
            {
                // 20051 reconnect
                // 20060 maintanance, pause
                // 20061 maintanance end, resub
            }
        }

        private void HandleSubscriptionEvent(BitfinexSubscriptionResponse subscription)
        {
            BitfinexEventRegistration pending;
            lock(eventListLock)
                pending = eventRegistrations.SingleOrDefault(r => r.ChannelName == subscription.ChannelName && !r.Confirmed);

            if (pending == null) {
                Log.Write(LogVerbosity.Warning, "Received registration confirmation but have nothing pending?");
                return;
            }

            pending.ChannelId = subscription.ChannelId;
            pending.Confirmed = true;
            Log.Write(LogVerbosity.Info, $"Subscription confirmed for channel {subscription.ChannelName}, ID: {subscription.ChannelId}");
        }

        private void HandleErrorEvent(BitfinexSocketError error)
        {
            Log.Write(LogVerbosity.Warning, $"Bitfinex socket error: {error.ErrorCode} - {error.ErrorMessage}");
            BitfinexEventRegistration waitingRegistration;
            lock (eventListLock) 
                waitingRegistration = eventRegistrations.SingleOrDefault(e => !e.Confirmed);

            if (waitingRegistration != null)
                waitingRegistration.Error = new BitfinexError(error.ErrorCode, error.ErrorMessage);
        }

        private void HandleUnhandledEvent(JObject data)
        {
            Log.Write(LogVerbosity.Info, $"Received uknown event: { data }");
        }

        private void HandleAccountEvent(BitfinexSocketEvent evnt)
        {
            if (evnt.Event == WalletsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexWallet[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexWalletSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == OrdersSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexOrder[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexOrderSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == PositionsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexPosition[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexPositionsSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingOffersSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingOffer[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingOffersSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingCreditsSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingCredit[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingCreditsSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == FundingLoansSnapshotEvent)
            {
                var obj = evnt.Data.ToObject<BitfinexFundingLoan[]>();
                foreach (var handler in GetRegistrationsOfType<BitfinexFundingLoansSnapshotEventRegistration>())
                    handler.Handler(obj);
            }
            else if (evnt.Event == ActiveTradesSnapshotEvent)
            {
            }
            else
            {
                Log.Write(LogVerbosity.Warning, $"Received unknown account event: {evnt.Event}, data: {evnt.Data}");
            }
        }

        private void HandleChannelEvent(JArray evnt)
        {
            var eventId = (int)evnt[0];

            BitfinexEventRegistration registration;
            lock (eventListLock)
                registration = eventRegistrations.SingleOrDefault(s => s.ChannelId == eventId);

            if(registration == null)
            {
                Log.Write(LogVerbosity.Warning, $"Received event but have no registration (eventId={eventId})");
                return;
            }

            if (registration is BitfinexTradingPairTickerEventRegistration)
                ((BitfinexTradingPairTickerEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexSocketTradingPairTick>());

            if (registration is BitfinexFundingPairTickerEventRegistration)
                ((BitfinexFundingPairTickerEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexSocketFundingPairTick>());

            if (registration is BitfinexOrderBooksEventRegistration)
            {
                var evnt1 = evnt[1];
                if (evnt1 is JArray)
                {
                    BitfinexSocketOrderBook[] bookArray;
                    if (evnt1[0] is JArray)
                    {
                        bookArray = evnt1.ToObject<BitfinexSocketOrderBook[]>();
                    }
                    else
                    {
                       var book = evnt1.ToObject<BitfinexSocketOrderBook>();
                        bookArray = new BitfinexSocketOrderBook[] { book };
                    }
                    ((BitfinexOrderBooksEventRegistration)registration).Handler(bookArray);
                }
            }

            if (registration is BitfinexTradeEventRegistration)
            {
                if(evnt[1] is JArray)
                    ((BitfinexTradeEventRegistration)registration).Handler(evnt[1].ToObject<BitfinexTradeSimple[]>());
                else
                    ((BitfinexTradeEventRegistration)registration).Handler(new[] { evnt[2].ToObject<BitfinexTradeSimple>() });
            }
        }

        public long SubscribeToOrdersSnapshot(Action<BitfinexOrder[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexOrderSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = OrdersSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToWalletSnapshotEvent(Action<BitfinexWallet[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexWalletSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = WalletsSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToPositionsSnapshotEvent(Action<BitfinexPosition[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexPositionsSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                EventTypes = new List<string>() { PositionsSnapshotEvent },
                Handler = handler
            });

            return id;
        }

        public long SubscribeToFundingOffersSnapshotEvent(Action<BitfinexFundingOffer[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingOffersSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingOffersSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public long SubscribeToFundingCreditsSnapshotEvent(Action<BitfinexFundingCredit[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingCreditsSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingCreditsSnapshotEvent,
                Handler = handler
            });

            return id;
        }
        
        public long SubscribeToFundingLoansSnapshotEvent(Action<BitfinexFundingLoan[]> handler)
        {
            long id = registrationId;
            AddEventRegistration(new BitfinexFundingLoansSnapshotEventRegistration()
            {
                Id = id,
                Confirmed = true,
                ChannelName = FundingLoansSnapshotEvent,
                Handler = handler
            });

            return id;
        }

        public BitfinexApiResult<long> SubscribeToOrderBooks(string symbol, Action<BitfinexSocketOrderBook[]> handler, Precision precision = Precision.P0, Frequency frequency = Frequency.F0, int length = 25)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexOrderBooksEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "book",
                    Handler = handler
                };
                AddEventRegistration(registration);

                var msg = JsonConvert.SerializeObject(new BitfinexBookSubscribeRequest(symbol, precision.ToString(), frequency.ToString(), length));
                socket.Send(msg);

                return WaitSubscription(registration);
            }
        }

        public BitfinexApiResult<long> SubscribeToTradingPairTicker(string symbol, Action<BitfinexSocketTradingPairTick> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexTradingPairTickerEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "ticker",
                    Handler = handler
                };
                AddEventRegistration(registration);
                
                socket.Send(JsonConvert.SerializeObject(new BitfinexTickerSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        public BitfinexApiResult<long> SubscribeToFundingPairTicker(string symbol, Action<BitfinexSocketFundingPairTick> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexFundingPairTickerEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "ticker",
                    Handler = handler
                };
                AddEventRegistration(registration);
                
                socket.Send(JsonConvert.SerializeObject(new BitfinexTickerSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        public BitfinexApiResult<long> SubscribeToTrades(string symbol, Action<BitfinexTradeSimple[]> handler)
        {
            lock (subscriptionLock)
            {
                var registration = new BitfinexTradeEventRegistration()
                {
                    Id = registrationId,
                    ChannelName = "trades",
                    Handler = handler
                };
                AddEventRegistration(registration);

                socket.Send(JsonConvert.SerializeObject(new BitfinexTradeSubscribeRequest(symbol)));

                return WaitSubscription(registration);
            }
        }

        private BitfinexApiResult<long> WaitSubscription(BitfinexEventRegistration registration)
        {
            var sw = Stopwatch.StartNew();
            if (!registration.CompleteEvent.WaitOne(3000))
            {
                lock(eventListLock)
                    eventRegistrations.Remove(registration);
                return ThrowErrorMessage<long>(BitfinexErrors.GetError(BitfinexErrorKey.SubscriptionNotConfirmed));
            }
            sw.Stop();
            Log.Write(LogVerbosity.Info, $"Wait took {sw.ElapsedMilliseconds}ms");

            if (registration.Confirmed)
                return ReturnResult(registration.Id);

            lock(eventListLock)
                eventRegistrations.Remove(registration);
            return ThrowErrorMessage<long>(registration.Error);            
        }

        private void AddEventRegistration(BitfinexEventRegistration reg)
        {
            lock (eventListLock)
                eventRegistrations.Add(reg);
        }

        private IEnumerable<T> GetRegistrationsOfType<T>()
        {
            lock(eventListLock)
                return eventRegistrations.OfType<T>();
        }
    }
}

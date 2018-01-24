using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Logging;
using Bitfinex.Json.Objects;
using System.Globalization;

namespace Bitfinex
{
    public abstract class BitfinexAbstractApi: IDisposable
    {
        protected string ApiKey { get; private set; }
        private HMACSHA384 m_encryptor;
        protected static Logger Log { get; } = new Logger();

        protected bool HasSecretKey => m_encryptor != null;

        protected BitfinexAbstractApi()
        {
//            if (BitfinexDefaults.LogWriter != null)
//                SetLogOutput(BitfinexDefaults.LogWriter);

//            if (BitfinexDefaults.LogVerbosity != null)
//                SetLogVerbosity(BitfinexDefaults.LogVerbosity.Value);

//            if (BitfinexDefaults.ApiKey != null && BitfinexDefaults.ApiSecret != null)
//                SetApiCredentials(BitfinexDefaults.ApiKey, BitfinexDefaults.ApiSecret);
        }

        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetApiKey(apiKey);
            SetApiSecret(apiSecret);
        }

        /// <summary>
        /// Sets the API Key. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiKey">The api key</param>
        public void SetApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Api key empty");

            this.ApiKey = apiKey;
        }

        /// <summary>
        /// Sets the API Secret. Api keys can be managed at https://bittrex.com/Manage#sectionApi
        /// </summary>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiSecret(string apiSecret)
        {
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("Api secret empty");

            m_encryptor = new HMACSHA384(Encoding.ASCII.GetBytes(apiSecret));
        }

        /// <summary>
        /// Sets the verbosity of the log messages
        /// </summary>
        /// <param name="verbosity">Verbosity level</param>
        public static void SetLogVerbosity(LogVerbosity verbosity)
        {
            Log.Level = verbosity;
        }

        /// <summary>
        /// Sets the log output
        /// </summary>
        /// <param name="writer">The output writer</param>
        public static void SetLogOutput(TextWriter writer)
        {
            Log.TextWriter = writer;
        }

        protected static BitfinexApiResult<T> ThrowErrorMessage<T>(BitfinexError error)
        {
            return ThrowErrorMessage<T>(error, null);
        }

        protected static BitfinexApiResult<T> ThrowErrorMessage<T>(BitfinexError error, string extraInformation)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Log.Write(LogVerbosity.Warning, $"Call failed: {error.ErrorMessage}");
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Error = error;
            if (extraInformation != null)
                result.Error.ErrorMessage += Environment.NewLine + extraInformation;
            return result;
        }

        protected static BitfinexApiResult<T> ReturnResult<T>(T data)
        {
            var result = (BitfinexApiResult<T>)Activator.CreateInstance(typeof(BitfinexApiResult<T>));
            result.Result = data;
            result.Success = true;
            return result;
        }

        private static string ByteToString(byte[] buff)
        {
            var sbinary = "";
            foreach (byte t in buff)
                sbinary += t.ToString("x2", CultureInfo.InvariantCulture); /* hex format */
            return sbinary;
        }

        protected string GetHexHashSignature(string payload)
        {
            return ByteToString(m_encryptor.ComputeHash(Encoding.ASCII.GetBytes(payload)));
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
                if(m_encryptor != null)
                {
                    m_encryptor.Dispose();
                    m_encryptor = null;
                }
            }
        }
    }
}

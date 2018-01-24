using Bitfinex.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Json.Objects
{
    [JsonConverter(typeof(BitfinexResultConverter))]
    public class BitfinexError
    {
        [BitfinexProperty(1)]
        public int ErrorCode { get; set; }
        [BitfinexProperty(2)]
        public string ErrorMessage { get; set; }

        public BitfinexError(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}

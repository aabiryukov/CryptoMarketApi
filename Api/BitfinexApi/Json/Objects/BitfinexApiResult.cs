using Newtonsoft.Json;

namespace Bitfinex.Json.Objects
{
    /// <summary>
    /// The result of an Api call
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    public class BitfinexApiResult<T>
    {
        /// <summary>
        /// Whether the Api call was successful
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; internal set; }
        /// <summary>
        /// The result of the Api call
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; internal set; }
        /// <summary>
        /// The message if the call wasn't successful
        /// </summary>
        [JsonProperty("message")]
        public BitfinexError Error { get; internal set; }

        public override string ToString()
        {
            if (!Success)
                return $"Error: {Error.ErrorMessage}";

            return $"Success: {Result}";
        }
    }
}

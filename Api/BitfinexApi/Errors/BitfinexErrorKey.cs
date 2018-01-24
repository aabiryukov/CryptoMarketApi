namespace Bitfinex.Errors
{
    public enum BitfinexErrorKey
    {
        NoApiCredentialsProvided,
        InputValidationFailed,

        ParseErrorReader,
        ParseErrorSerialization,

        ErrorWeb,
        CannotConnectToServer,
        WithdrawFailed,
        DepositAddressFailed,

        SubscriptionNotConfirmed,

        UnknownError
    }
}

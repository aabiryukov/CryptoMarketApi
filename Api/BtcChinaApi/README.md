#C# Library of BTCChina Trade API
A C# wrapper class to trade bitcoin and litecoin via [BTCChina](https://www.btcchina.com) API.

##Installation

1. Download the library.
2. Add BTCChinaAPI.cs and BTCChinaException.cs to your project.
3. Add BTCChina namespace.

##Usage
Create Trade API keys at https://vip.btcchina.com/account/apikeys, and set proper permissions as indicated.

Spawn BTCChinaAPI instance with access key and secret key mentioned above. Notice that these keys cannot be modified later.

```C#
BTCChinaAPI btcAPI = new BTCChinaAPI(access_key, secret_key);
```

Call methods similiar to the format described in [API documentation](http://btcchina.org/api-trade-documentation-en). Results are always returned as string.

```C#
string result = btcAPI.getAccountInfo();
```
##Exceptions
The BTCChinaException class has three string attributes:

- _Message_: body of error message.
- _RequestMethod_: name of the method in which the exception occurred.
- _RequestID_: JSON request ID of the remote API call.

Notice that JSON request ID has been wrapped and used exclusively inside DoMethod() after the recent update, so this attribute may not make any sense.

##Enumerations
Enumerations can be iterated by _BTCChinaAPI.TypeName.MemberName_.
 
 - MarketType: BTCCNY | LTCCNY | LTCBTC | ALL
 - CurrencyType: BTC | LTC
 - TransactionType: all | fundbtc | withdrawbtc | fundmoney | withdrawmoney | refundmoney | buybtc | sellbtc | buyltc | sellltc | tradefee | rebate

##Examples
###Get user information
```C#
result = btcAPI.getAccountInfo();
```

_Result_:
JSON Objects of [profile](http://btcchina.org/api-trade-documentation-en#profile), [balance](http://btcchina.org/api-trade-documentation-en#balance) and [frozen](http://btcchina.org/api-trade-documentation-en#frozen).

###Place order
```C#
result = btcAPI.placeOrder(double price, double amount, MarketType market = MarketType.BTCCNY);
```

Market type determines the precision of price and amount. See [FAQ](http://btcchina.org/api-trade-documentation-en#faq) No.6 for details.
_Parameters:_

- _price_: negative value to sell or buy at market price.
- _amount_: negative value to sell while positive value to buy.
- _market_: the market to place this order. Notice that ALL is not supported.

_Result:_
{"result":orderID} on success. [Invalid amount or invalid price](http://btcchina.org/api-trade-documentation-en#error_codes) error may occur.

###Cancel order
```C#
result = btcAPI.cancelOrder(int orderID, MarketType market = MarketType.BTCCNY);
```
_Parameters:_

- _orderID_: the ID returned by placeOrder method
- _market_: the market of the order placed previously. Notice that ALL is not supported.

_Result_:
{"result":true} if successful, otherwise {"result":false}

###Get Market Depth
```C#
result = btcAPI.getMarketDepth(unsigned int limit = 10, MarketType market = MarketType.BTCCNY);
```

Get the complete market depth.
_Parameters:_

- _limit_: number of orders returned per side.
- _market_: the market to get depth of. Notice that ALL is not supported.

_Result:_
[market_depth](http://btcchina.org/api-trade-documentation-en#market_depth) JSON object.

###Get Deposits
```C#
result = btcAPI.getDeposits(CurrencyType currency, bool pendingonly = true);
```

Get all user deposits.

_Parameters:_

- _currency_: type of currency to get deposit records of.
- _pendingonly_: whether to get open deposits only.

_Result:_
Array of [deposit](http://btcchina.org/api-trade-documentation-en#deposit) JSON objects.

###Get Withdrawals
```C#
result = btcAPI.getWithdrawals(CurrencyType currency, bool pendingonly = true);
```

Get all user withdrawals.

_Parameters:_

- _currency_: type of currency to get deposit records of.
- _pendingonly_: whether to get open withdrawals only.

_Result:_
[withdrawal](http://btcchina.org/api-trade-documentation-en#withdrawal) JSON object.

###Get single withdrawal status
```C#
result = btcAPI.getWithdrawal(int withdrawalID, CurrencyType currency = CurrencyType.BTC);
```

_Parameters:_

- _withdrawalID_: the withdrawal to get status of.
- _currency_: type of currency.

_Result:_
[withdrawal](http://btcchina.org/api-trade-documentation-en#withdrawal) JSON object.

###Request a withdrawal
```C#
result = btcAPI.requestWithdrawal(CurrencyType currency, double amount);
```

Make a withdrawal request. BTC withdrawals will pick last used withdrawal address from user profile.

_Parameters:_

- _currency_: type of currency to withdraw.
- _amount_: amount of currency to withdraw

_Result:_
{"result":{"id":"withdrawalID"}}
Notice that the return format of withdrawalID is different from that of orderID.

###Get order status
```C#
result = btcAPI.getOrder(unsigned int orderID, MarketType market = MarketType.BTCCNY);
```

_Parameters:_

- _orderID_: the order to get status of.
- _market_: the market in which the order is placed. Notice that ALL is not supported.

_Result:_
[order](http://btcchina.org/api-trade-documentation-en#order) JSON object.

###Get all order status
```C#
result = btcAPI.getOrders(bool openonly = true, MarketType market = MarketType.BTCCNY, unsigned int limit = 1000, unsigned int offset = 0);
```

_Parameters:_

- _openonly_: whether to get open orders only.
- _market_: the market in which orders are placed.
- _limit_: the number of orders to show.
- _offset_: page index of orders.

_Result:_
Array of [order](http://btcchina.org/api-trade-documentation-en#order) JSON objects.

###Get transaction log
```C#
result = btcAPI.getTransactions(TransactionType transaction = TransactionType.all, unsigned int limit = 10, unsigned int offset = 0);
```

Notice that prices returned by this method may differ from placeOrder as it is the price get procceeded.

_Parameters:_

- _transaction_: type of transaction to fetch.
- _limit_: the number ot transactions.
- _offset_: page index ot transactions.

_Result:_
Array of [transaction](http://btcchina.org/api-trade-documentation-en#transaction) JSON objects.




> Written with [StackEdit](https://stackedit.io/).
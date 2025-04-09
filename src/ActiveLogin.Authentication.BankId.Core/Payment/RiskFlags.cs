namespace ActiveLogin.Authentication.BankId.Core.Payment;
public enum RiskFlags
{
    newCard,
    newCustomer,
    newRecipient,
    highRiskRecipient,
    largeAmount,
    foreignCurrency,
    cryptoCurrencyPurchase,
    moneyTransfer,
    overseasTransaction,
    recurringPayment,
    suspiciousPaymentPattern,
    other
}

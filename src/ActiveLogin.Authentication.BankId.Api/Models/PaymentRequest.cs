namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Payment request parameters.
/// </summary>
public class PaymentRequest : Request
{
    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as it is seen by your service.
    /// IPv4 and IPv6 are allowed.
    /// Make sure that the IP address you include as endUserIp is the address of your end user's device, not the internal address of any reverse proxy between you and the end user.
    /// In use cases where the IP address is not available, e. g. for voice-based services, the internal representation of those systems' IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    /// <param name="returnUrl">The URL to return to when the authentication order is completed.</param>
    /// <param name="returnRisk">If set to true, a risk indication will be included in the collect response.</param>
    /// <param name="riskFlags">
    /// Indicate to the risk assessment system that the payment has a higher risk or is unusual for the user. List of String.
    /// Possible values: newCard, newCustomer, newRecipient, highRiskRecipient, largeAmount, foreignCurrency,
    /// cryptoCurrencyPurchase, moneyTransfer, overseasTransaction, recurringPayment, suspiciousPaymentPattern, other
    /// </param>
    /// <param name="userVisibleTransaction">Information about the transaction being approved.</param>
    /// <param name="web">Information about the app device the end user is using.</param>
    /// <param name="app">Information about the web browser the end user is using.</param>
    public PaymentRequest(
        string endUserIp,
        Requirement? requirement = null,
        string? userVisibleData = null,
        byte[]? userNonVisibleData = null,
        string? userVisibleDataFormat = null,
        string? returnUrl = null,
        bool? returnRisk = null,
        List<string>? riskFlags = null,
        UserVisibleTransaction? userVisibleTransaction = null,
        DeviceDataWeb? web = null,
        DeviceDataApp? app = null)
        : base(endUserIp,
            userVisibleData: userVisibleData,
            userNonVisibleData: userNonVisibleData,
            requirement: requirement,
            userVisibleDataFormat: userVisibleDataFormat,
            returnUrl: returnUrl,
            returnRisk: returnRisk,
            riskFlags: riskFlags,
            userVisibleTransaction: userVisibleTransaction,
            web: web,
            app: app)
    {
    }
}

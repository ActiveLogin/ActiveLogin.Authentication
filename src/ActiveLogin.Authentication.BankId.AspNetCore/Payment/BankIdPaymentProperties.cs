using ActiveLogin.Identity.Swedish;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.Payment;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdPaymentProperties
{
    /// <summary>
    /// The properties used for payment.
    /// </summary>
    /// <param name="transactionType">
    /// The type of transaction.
    /// The possible values have the following meaning:
    /// card: Card payment.
    /// npa: Non-payment authentication.
    /// </param>
    /// <param name="recipientName">
    /// The name of the recipient of the payment.
    /// For the transaction type "card", this is the merchant name.
    /// </param>
    public BankIdPaymentProperties(TransactionType transactionType, string recipientName)
    {
        TransactionType = transactionType;
        RecipientName = recipientName;
    }

    /// <summary>
    /// The type of transaction.
    /// The possible values have the following meaning:
    /// card: Card payment.
    /// npa: Non-payment authentication.
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// The name of the recipient of the payment.
    /// For the transaction type "card", this is the merchant name.
    /// </summary>
    public string RecipientName { get; set; }

    /// <summary>
    /// Object that sets monetary amount for the payment.
    /// If the transactionType is npa this isn't allowed to be set.
    /// </summary>
    public Money? Money { get; set; }

    /// <summary>
    /// Indicate to the user that the payment has higher risk or is unusual for the user.
    /// This will be indicated in the UI.
    /// </summary>
    public string? RiskWarning { get; set; }

    /// <summary>
    /// Indicate to the risk assessment system that the payment has a higher risk or is unusual for the user.
    /// </summary>
    public IEnumerable<RiskFlags>? RiskFlags { get; set; }

    /// <summary>
    /// Text displayed to the user during the order.
    /// The purpose is to provide context, thereby enabling the user to detect identification
    /// errors and avert fraud attempts.
    /// The text can be formatted using CR, LF and CRLF for new lines.The text must be encoded
    /// as UTF-8 and then base 64 encoded.
    /// </summary>
    public string? UserVisibleData { get; set; }

    /// <summary>
    /// If present and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters.
    /// The possible values have the following meaning:
    /// plaintext: userVisibleData contains base 64 encoded text using a sub-set of UTF-8 and CR, LF or CRLF for line breaks..
    /// simpleMarkdownV1: userVisibleData contains Simple Markdown version 1.
    /// Please see the documentation from BankId for more information about the format.
    /// </summary>
    public string? UserVisibleDataFormat { get; set; }

    /// <summary>
    /// Data that you wish to include but not display to the user.
    /// The value must be base 64-encoded.
    /// </summary>
    public byte[]? UserNonVisibleData { get; set; }

    /// <summary>
    /// The personal identity number allowed to confirm the sign request.
    /// If a BankID with another personal identity number attempts to confirm the sign request, it will fail.
    /// If left empty any personal identity number will be allowed.
    /// </summary>
    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; set; }

    /// <summary>
    /// Whether the user needs to confirm their identity with a valid Swedish passport or national ID card to complete the order.
    /// No identity confirmation is required by default.
    /// </summary>
    public bool? RequireMrtd { get; set; }

    /// <summary>
    /// Users are required to confirm the order with their security code even if they have biometrics activated.
    /// </summary>
    public bool? RequirePinCode { get; set; }

    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    public List<BankIdCertificatePolicy> BankIdCertificatePolicies { get; set; } = new();

    /// <summary>
    /// Whether the user needs to complete the order using a card reader for the signature.
    /// <para>The possible values have the following meaning:</para>
    /// <para>class1: The order must be confirmed with a card reader where the PIN code is entered on a computer keyboard, or a card reader of higher class.</para>
    /// <para>class2: The order must be confirmed with a card reader where the PIN code is entered on the reader.</para>
    /// <para>This condition should always be combined with a certificatePolicies for a smart card to avoid undefined behaviour.</para>
    /// <para>No card reader is required by default.</para>
    /// </summary>
    public CardReader? CardReader { get; set; }


    /// <summary>
    /// A collection of items where you can store state that will be provided once the sign flow is done.
    /// </summary>
    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}

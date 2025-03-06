using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignProperties
{
    /// <summary>
    /// The properties used for signing.
    /// </summary>
    /// <param name="userVisibleData">
    /// The text can be formatted using CR, LF and CRLF for new lines.
    /// The text must be encoded as UTF-8 and then base 64 encoded.
    /// 1—1 500 characters after base 64encoding.
    ///
    /// Scenario sign: The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
    ///
    /// Scenario auth: A text that is displayed to the user during authentication with BankID, with the
    /// purpose of providing context for the authentication and to enable users to notice if
    /// there is something wrong about the identification and avoid attempted frauds.
    /// </param>
    public BankIdSignProperties(string userVisibleData)
    {
        UserVisibleData = userVisibleData;
    }

    /// <summary>
    /// The text can be formatted using CR, LF and CRLF for new lines.
    /// The text must be encoded as UTF-8 and then base 64 encoded.
    /// 1—1 500 characters after base 64encoding.
    ///
    /// Scenario sign: The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
    ///
    /// Scenario auth: A text that is displayed to the user during authentication with BankID, with the
    /// purpose of providing context for the authentication and to enable users to notice if
    /// there is something wrong about the identification and avoid attempted frauds.
    /// </summary>
    public string UserVisibleData { get; set; }

    /// <summary>
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </summary>
    public string? UserVisibleDataFormat { get; set; }

    /// <summary>
    /// Data not displayed to the user.
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

using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignOptions
{
    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    public List<BankIdCertificatePolicy> BankIdCertificatePolicies { get; set; } = new();

    /// <summary>
    /// Users are required to sign the transaction with their PIN code, even if they have biometrics activated.
    /// </summary>
    public bool BankIdRequirePinCode { get; set; } = false;

    /// <summary>
    /// If present, and set to "true", the client needs to provide MRTD (Machine readable travel document)
    /// information to complete the order. Only Swedish passports and national ID cards are supported.
    /// </summary>
    public bool BankIdRequireMrtd { get; set; } = false;

    /// <summary>
    /// If this is set to true a risk indicator will be included in the collect response when the order completes.
    /// If a risk indicator is required for the order to complete, for example, if a risk requirement is applied,
    /// the returnRisk property is ignored, and a risk indicator is always included; otherwise a default value of
    /// false is used. The risk indication requires that the endUserIp is correct.
    /// </summary>
    public bool BankIdReturnRisk { get; set; } = false;

    /// <summary>
    /// Auto launch the BankID app on the current device.
    /// </summary>
    internal bool BankIdSameDevice { get; set; } = false;


    private CookieBuilder _stateCookieBuilder = new()
    {
        Name = BankIdConstants.DefaultStateCookieName,
        SecurePolicy = CookieSecurePolicy.SameAsRequest,
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        IsEssential = true
    };

    public CookieBuilder StateCookie
    {
        get => _stateCookieBuilder;
        set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Whether the user needs to complete the order using a card reader for the signature.
    /// <para>The possible values have the following meaning:</para>
    /// <para>class1: The order must be confirmed with a card reader where the PIN code is entered on a computer keyboard, or a card reader of higher class.</para>
    /// <para>class2: The order must be confirmed with a card reader where the PIN code is entered on the reader.</para>
    /// <para>This condition should always be combined with a certificatePolicies for a smart card to avoid undefined behaviour.</para>
    /// <para>No card reader is required by default.</para>
    /// </summary>
    public CardReader? CardReader { get; set; }
}

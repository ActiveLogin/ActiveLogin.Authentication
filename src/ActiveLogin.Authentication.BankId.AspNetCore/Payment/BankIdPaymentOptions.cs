using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Risk;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdPaymentOptions
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
    /// false is used. The risk indication requires that the endUserIp is correct. Please note that the assessed
    /// risk will not be returned if the order was blocked, which may happen if a risk requirement is set.
    /// </summary>
    public bool BankIdReturnRisk { get; set; } = false;

    /// <summary>
    /// Set the acceptable risk level for the transaction. If the risk is higher than the specified level,
    /// the transaction will be blocked. The risk indication requires that the endUserIp is correct.
    /// An incorrect IP-address will result in legitimate transactions being blocked.
    /// </summary>
    public BankIdAllowedRiskLevel BankIdAllowedRiskLevel { get; set; } = BankIdAllowedRiskLevel.NoRiskLevel;

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
}

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignOptions
{
    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    public List<string> BankIdCertificatePolicies { get; set; } = new();

    /// <summary>
    /// Users are required to sign the transaction with their PIN code, even if they have biometrics activated.
    /// </summary>
    public bool BankIdRequirePinCode { get; set; } = false;

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

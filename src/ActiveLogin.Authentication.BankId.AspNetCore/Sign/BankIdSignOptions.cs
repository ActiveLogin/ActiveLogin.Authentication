using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignOptions
{
    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    public List<string> BankIdCertificatePolicies { get; set; } = new();

    /// <summary>
    /// Users of iOS and Android devices may use fingerprint or face recognition for authentication if the device supports it and the user configured the device to use it.
    /// </summary>
    public bool BankIdAllowBiometric { get; set; } = true;

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

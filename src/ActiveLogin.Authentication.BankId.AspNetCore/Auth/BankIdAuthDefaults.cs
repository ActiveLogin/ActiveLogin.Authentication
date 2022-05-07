using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

/// <summary>
/// Default values used for auth configuration.
/// </summary>
public static class BankIdAuthDefaults
{
    public const string IdentityProviderName = "BankID";
    public const string AuthenticationMethodName = "bankid";
    public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

    public const string SameDeviceAuthenticationScheme = "bankid-samedevice";
    public const string SameDeviceDisplayName = "BankID (Denna enhet)";
    public static readonly PathString SameDeviceCallbackPath = new("/signin-bankid-samedevice");

    public const string OtherDeviceAuthenticationScheme = "bankid-otherdevice";
    public const string OtherDeviceDisplayName = "BankID (Annan enhet)";
    public static readonly PathString OtherDeviceCallbackPath = new("/signin-bankid-otherdevice");
}

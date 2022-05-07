using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

/// <summary>
/// Default values used for sign configuration.
/// </summary>
public static class BankIdSignDefaults
{
    public const string SameDeviceConfigKey = "bankid-samedevice";
    public const string SameDeviceConfigDisplayName = "BankID (Denna enhet)";
    public static readonly PathString SameDeviceCallbackPath = new("/sign-bankid-samedevice");

    public const string OtherDeviceConfigKey = "bankid-otherdevice";
    public const string OtherDeviceConfigDisplayName = "BankID (Annan enhet)";
    public static readonly PathString OtherDeviceCallbackPath = new("/sign-bankid-otherdevice");
}

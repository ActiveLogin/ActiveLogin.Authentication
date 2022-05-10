namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

/// <summary>
/// Default values used for sign configuration.
/// </summary>
public static class BankIdSignDefaults
{
    public const string SameDeviceConfigKey = "bankid-samedevice";
    public const string SameDeviceConfigDisplayName = "BankID (Denna enhet)";

    public const string OtherDeviceConfigKey = "bankid-otherdevice";
    public const string OtherDeviceConfigDisplayName = "BankID (Annan enhet)";
}

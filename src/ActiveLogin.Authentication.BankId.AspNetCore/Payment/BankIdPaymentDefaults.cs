namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

/// <summary>
/// Default values used for payment configuration.
/// </summary>
public static class BankIdPaymentDefaults
{
    public const string SameDeviceConfigKey = "al-bankid-payment-samedevice";
    public const string SameDeviceConfigDisplayName = "BankID (Denna enhet)";

    public const string OtherDeviceConfigKey = "al-bankid-payment-otherdevice";
    public const string OtherDeviceConfigDisplayName = "BankID (Annan enhet)";
}

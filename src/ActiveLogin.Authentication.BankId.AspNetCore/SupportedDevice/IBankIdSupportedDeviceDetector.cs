namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    /// <summary>
    ///     Detect if the device requesting is one of the supported platforms for BankID.
    /// </summary>
    public interface IBankIdSupportedDeviceDetector
    {
        BankIdSupportedDevice Detect(string userAgent);
    }
}

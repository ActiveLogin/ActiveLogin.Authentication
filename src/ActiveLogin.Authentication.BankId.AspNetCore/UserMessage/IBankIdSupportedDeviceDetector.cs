using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserMessage
{
    /// <summary>
    /// Detect if the device requesting is one of the supported platforms for BankID.
    /// </summary>
    public interface IBankIdSupportedDeviceDetector
    {
        BankIdSupportedDevice Detect(string userAgent);
    }
}
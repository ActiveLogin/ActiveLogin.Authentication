namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    /// <summary>
    /// Detect if the device requesting is one of the supported platforms for BankID.
    /// </summary>
    public interface IBankIdSupportedDeviceDetector
    {
        /// <summary>
        /// Detect the device.
        /// </summary>
        /// <param name="userAgent">The user agent string to extract device information from.</param>
        /// <returns></returns>
        BankIdSupportedDevice Detect(string userAgent);
    }
}

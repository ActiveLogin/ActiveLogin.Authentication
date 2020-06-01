namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public enum BankIdSupportedDeviceBrowser
    {
        Unknown,

        Chrome,
        Safari,
        Firefox,
        Edge,
        SamsungBrowser,
        Opera,

        // Brave - Brave behaves the exact same way as Chrome (for privacy reasons), so can't be detected
    }
}

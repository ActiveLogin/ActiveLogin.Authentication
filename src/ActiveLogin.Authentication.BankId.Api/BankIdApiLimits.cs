namespace ActiveLogin.Authentication.BankId.Api;

public static class BankIdApiLimits
{
    /// <summary>
    /// Max length for web.userAgent used in Auth/Sign/Payment as defined by the BankID API.
    /// </summary>
    public const int UserAgentMaxLength = 256;
}

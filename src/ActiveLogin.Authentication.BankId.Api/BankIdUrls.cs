namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// URLs for BankID REST API.
/// </summary>
public static class BankIdUrls
{
    public const string AppApiVersion = "6.0";
    public const string VerifyApiVersion = "1";


    /// <summary>
    /// Base url for production API. Needs to be used in conjunction with a production certificate.
    /// </summary>
    public static readonly Uri AppApiProductionBaseUrl = new($"https://appapi2.bankid.com/rp/v{AppApiVersion}/");

    /// <summary>
    /// Base url for test API. Needs to be used in conjunction with the test certificate.
    /// </summary>
    public static readonly Uri AppApiTestBaseUrl = new($"https://appapi2.test.bankid.com/rp/v{AppApiVersion}/");



    /// <summary>
    /// Base url for production API. Needs to be used in conjunction with a production certificate.
    /// </summary>
    public static readonly Uri VerifyApiProductionBaseUrl = new($"https://idcardapi.bankid.com/rp/v{AppApiVersion}/");

    /// <summary>
    /// Base url for test API. Needs to be used in conjunction with the test certificate.
    /// </summary>
    public static readonly Uri VerifyApiTestBaseUrl = new($"https://idcardapi.test.bankid.com/rp/v{AppApiVersion}/");
}

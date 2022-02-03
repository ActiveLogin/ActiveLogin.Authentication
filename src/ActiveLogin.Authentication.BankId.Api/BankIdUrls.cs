using System;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    /// URLs for BankID REST API.
    /// </summary>
    public static class BankIdUrls
    {
        /// <summary>
        /// Base url for production API. Needs to be used in conjunction with a production certificate.
        /// </summary>
        public static readonly Uri ProductionApiBaseUrl = new Uri("https://appapi2.bankid.com/rp/v5.1/");

        /// <summary>
        /// Base url for test API. Needs to be used in conjunction with the test certificate.
        /// </summary>
        public static readonly Uri TestApiBaseUrl = new Uri("https://appapi2.test.bankid.com/rp/v5.1/");
    }
}

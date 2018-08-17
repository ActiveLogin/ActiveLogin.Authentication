using System;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// URLs for BankID REST API.
    /// </summary>
    public static class GrandIdUrls
    {
        /// <summary>
        /// Base url for production API. Needs to be used in conjunction with a production certificate.
        /// </summary>
        public static readonly Uri ProdApiBaseUrl = new Uri("https://client.grandid.com/json1.1/");

        /// <summary>
        /// Base url for test API. Needs to be used in conjunction with the test certificate.
        /// </summary>
        public static readonly Uri TestApiBaseUrl = new Uri("https://client-test.grandid.com/json1.1/");
    }
}
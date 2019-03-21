using System;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// URLs for GrandID REST API.
    /// </summary>
    public static class GrandIdUrls
    {
        private const string GrandIdProductionApiBaseUrl = "https://client.grandid.com/json1.1/";
        private const string GrandIdTestApiBaseUrl = "https://client-test.grandid.com/json1.1/";

        /// <summary>
        /// Base url for production API.
        /// </summary>
        public static readonly Uri ProductionApiBaseUrl = new Uri(GrandIdProductionApiBaseUrl);

        /// <summary>
        /// Base url for test API.
        /// </summary>
        public static readonly Uri TestApiBaseUrl = new Uri(GrandIdTestApiBaseUrl);
    }
}
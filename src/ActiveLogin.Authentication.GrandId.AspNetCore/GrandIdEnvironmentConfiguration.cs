using System;
using ActiveLogin.Authentication.GrandId.Api;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdEnvironmentConfiguration : IGrandIdEnvironmentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = GrandIdUrls.ProductionApiBaseUrl;
        public string ApiKey { get; set; }
    }
}
using System;
using ActiveLogin.Authentication.GrandId.Api;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdEnvironmentConfiguration : IGrandIdEnviromentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = GrandIdUrls.ProdApiBaseUrl;
        public string ApiKey { get; set; }
    }
}
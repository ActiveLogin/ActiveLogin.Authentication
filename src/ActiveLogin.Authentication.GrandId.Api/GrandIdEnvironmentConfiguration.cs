using System;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    public class GrandIdEnvironmentConfiguration : IGrandIdEnviromentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = GrandIdUrls.ProdApiBaseUrl;
        public string ApiKey { get; set; }

        public string SameDeviceServiceKey { get; set; }
        public string OtherDeviceServiceKey { get; set; }
        public string ChooseDeviceServiceKey { get; set; }
    }
}
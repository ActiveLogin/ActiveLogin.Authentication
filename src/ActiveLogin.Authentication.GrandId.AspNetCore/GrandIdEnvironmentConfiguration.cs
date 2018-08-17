using System;
using ActiveLogin.Authentication.GrandId.Api;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdEnvironmentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = GrandIdUrls.ProdApiBaseUrl;
        public string ApiKey { get; set; }
        public string SameDeviceServiceKey { get; set; }
        public string OtherDeviceServiceKey { get; set; }
        public string ChooseDeviceServiceKey { get; set; }

    }
}
using System;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdEnvironmentConfiguration : IGrandIdEnviromentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = GrandIdUrls.ProdApiBaseUrl;
        public string ApiKey { get; set; }
        public string SameDeviceServiceKey { get; set; }
        public string OtherDeviceServiceKey { get; set; }
        public string ChooseDeviceServiceKey { get; set; }

        public string GetDeviceOptionKey(DeviceOption deviceOption)
        {
            var deviceOptionKey = "";

            switch (deviceOption)
            {
                case DeviceOption.SameDevice:
                    deviceOptionKey = SameDeviceServiceKey;
                    break;
                case DeviceOption.OtherDevice:
                    deviceOptionKey = OtherDeviceServiceKey;
                    break;
                case DeviceOption.ChooseDevice:
                    deviceOptionKey = ChooseDeviceServiceKey;
                    break;
            }
            return deviceOptionKey;
        }
    }
}
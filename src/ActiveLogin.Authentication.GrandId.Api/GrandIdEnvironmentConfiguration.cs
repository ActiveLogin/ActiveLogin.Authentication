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

        public string GetDeviceOptionKey(DeviceOption deviceOption)
        {
            switch (deviceOption)
            {
                case DeviceOption.SameDevice:
                    return SameDeviceServiceKey;
                case DeviceOption.OtherDevice:
                    return OtherDeviceServiceKey;
                case DeviceOption.ChooseDevice:
                    return ChooseDeviceServiceKey;
                default:
                    throw new ArgumentException("Invalid device option", nameof(deviceOption));
            }
        }
    }
}
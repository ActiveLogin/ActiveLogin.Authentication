using ActiveLogin.Authentication.GrandId.Api.Models;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public interface IGrandIdEnviromentConfiguration
    {
        Uri ApiBaseUrl { get; set; }
        string ApiKey { get; set; }
        string SameDeviceServiceKey { get; set; }
        string OtherDeviceServiceKey { get; set; }
        string ChooseDeviceServiceKey { get; set; }

        string GetDeviceOptionKey(DeviceOption deviceOption);
    }
}
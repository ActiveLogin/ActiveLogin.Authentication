using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public interface IGrandIdEnvironmentConfiguration
    {
        Uri ApiBaseUrl { get; set; }
        string ApiKey { get; set; }
    }
}
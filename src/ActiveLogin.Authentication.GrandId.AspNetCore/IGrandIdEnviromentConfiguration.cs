using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public interface IGrandIdEnviromentConfiguration
    {
        Uri ApiBaseUrl { get; set; }
        string ApiKey { get; set; }
        string AuthenticateServiceKey { get; set; }
    }
}
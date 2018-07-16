using System;
using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdEnvironmentConfiguration
    {
        public Uri ApiBaseUrl { get; set; } = BankIdUrls.ProdApiBaseUrl;
    }
}
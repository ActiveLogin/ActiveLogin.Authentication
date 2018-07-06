using System;

namespace ActiveLogin.Authentication.BankId.Api
{
    public static class BankIdUrls
    {
        public static readonly Uri ProdApiBaseUrl = new Uri("https://appapi2.bankid.com/rp/v5/");
        public static readonly Uri TestApiBaseUrl = new Uri("https://appapi2.test.bankid.com/rp/v5/");
    }
}
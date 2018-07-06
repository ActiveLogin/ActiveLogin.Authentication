using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationPostConfigureOptions : IPostConfigureOptions<BankIdAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public BankIdAuthenticationPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        public void PostConfigure(string name, BankIdAuthenticationOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (options.StateDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector(
                    typeof(BankIdAuthenticationHandler).FullName,
                    name,
                    "v1"
                );

                options.StateDataFormat = new SecureDataFormat<BankIdState>(
                    new BankIdStateSerializer(),
                    dataProtector
                );
            }
        }
    }
}
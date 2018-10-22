using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection
{
    internal class BankIdLoginOptionsProtector : IBankIdLoginOptionsProtector
    {
        private readonly ISecureDataFormat<BankIdLoginOptions> _secureDataFormat;

        public BankIdLoginOptionsProtector(IDataProtectionProvider dataProtectionProvider)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(
                typeof(BankIdLoginResultProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<BankIdLoginOptions>(
                new BankIdLoginOptionsSerializer(),
                dataProtector
            );
        }

        public string Protect(BankIdLoginOptions loginOptions)
        {
            return _secureDataFormat.Protect(loginOptions);
        }

        public BankIdLoginOptions Unprotect(string protectedLoginOptions)
        {
            return _secureDataFormat.Unprotect(protectedLoginOptions);
        }
    }
}
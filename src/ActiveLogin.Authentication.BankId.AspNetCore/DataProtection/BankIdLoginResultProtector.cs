using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection
{
    internal class BankIdLoginResultProtector : IBankIdLoginResultProtector
    {
        private readonly ISecureDataFormat<BankIdLoginResult> _secureDataFormat;

        public BankIdLoginResultProtector(IDataProtectionProvider dataProtectionProvider)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(
                typeof(BankIdLoginResultProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<BankIdLoginResult>(
                new BankIdLoginResultSerializer(),
                dataProtector
            );
        }

        public string Protect(BankIdLoginResult loginResult)
        {
            return _secureDataFormat.Protect(loginResult);
        }

        public BankIdLoginResult Unprotect(string protectedLoginResult)
        {
            return _secureDataFormat.Unprotect(protectedLoginResult);
        }
    }
}
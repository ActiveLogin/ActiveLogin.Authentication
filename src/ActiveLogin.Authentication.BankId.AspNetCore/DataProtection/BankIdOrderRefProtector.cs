using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection
{
    internal class BankIdOrderRefProtector : IBankIdOrderRefProtector
    {
        private readonly ISecureDataFormat<BankIdOrderRef> _secureDataFormat;

        public BankIdOrderRefProtector(IDataProtectionProvider dataProtectionProvider)
        {
            IDataProtector dataProtector = dataProtectionProvider.CreateProtector(
                typeof(BankIdLoginResultProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<BankIdOrderRef>(
                new BankIdOrderRefSerializer(),
                dataProtector
            );
        }

        public string Protect(BankIdOrderRef orderRef)
        {
            return _secureDataFormat.Protect(orderRef);
        }

        public BankIdOrderRef Unprotect(string protectedOrderRef)
        {
            return _secureDataFormat.Unprotect(protectedOrderRef);
        }
    }
}

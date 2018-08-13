using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public class GrandIdOrderRefProtector : IGrandIdOrderRefProtector
    {
        private readonly ISecureDataFormat<GrandIdOrderRef> _secureDataFormat;

        public GrandIdOrderRefProtector(IDataProtectionProvider dataProtectionProvider)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(
                typeof(GrandIdLoginResultProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<GrandIdOrderRef>(
                new GrandIdOrderRefSerializer(),
                dataProtector
            );
        }

        public string Protect(GrandIdOrderRef orderRef)
        {
            return _secureDataFormat.Protect(orderRef);
        }

        public GrandIdOrderRef Unprotect(string protectedOrderRef)
        {
            return _secureDataFormat.Unprotect(protectedOrderRef);
        }
    }
}
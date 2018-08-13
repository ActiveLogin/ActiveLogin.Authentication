using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public class GrandIdLoginResultProtector : IGrandIdLoginResultProtector
    {
        private readonly ISecureDataFormat<GrandIdLoginResult> _secureDataFormat;

        public GrandIdLoginResultProtector(IDataProtectionProvider dataProtectionProvider)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(
                typeof(GrandIdLoginResultProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<GrandIdLoginResult>(
                new GrandIdLoginResultSerializer(),
                dataProtector
            );
        }

        public string Protect(GrandIdLoginResult loginResult)
        {
            return _secureDataFormat.Protect(loginResult);
        }

        public GrandIdLoginResult Unprotect(string protectedLoginResult)
        {
            return _secureDataFormat.Unprotect(protectedLoginResult);
        }
    }
}
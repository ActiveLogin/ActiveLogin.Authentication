using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection
{
    public class GrandIdSessionIdProtector : IGrandIdOrderRefProtector
    {
        private readonly ISecureDataFormat<GrandIdSessionId> _secureDataFormat;

        public GrandIdSessionIdProtector(IDataProtectionProvider dataProtectionProvider)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(
                typeof(GrandIdSessionIdProtector).FullName,
                "v1"
            );

            _secureDataFormat = new SecureDataFormat<GrandIdSessionId>(
                new GrandIdSessionIdSerializer(),
                dataProtector
            );
        }

        public string Protect(GrandIdSessionId sessionId)
        {
            return _secureDataFormat.Protect(sessionId);
        }

        public GrandIdSessionId Unprotect(string protectedSessionId)
        {
            return _secureDataFormat.Unprotect(protectedSessionId);
        }
    }
}
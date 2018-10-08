using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationPostConfigureOptions : IPostConfigureOptions<GrandIdAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public GrandIdAuthenticationPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        public void PostConfigure(string name, GrandIdAuthenticationOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (options.StateDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector(
                    typeof(GrandIdAuthenticationHandler).FullName,
                    name,
                    "v1"
                );

                options.StateDataFormat = new SecureDataFormat<GrandIdState>(
                    new GrandIdStateSerializer(),
                    dataProtector
                );
            }
        }
    }
}
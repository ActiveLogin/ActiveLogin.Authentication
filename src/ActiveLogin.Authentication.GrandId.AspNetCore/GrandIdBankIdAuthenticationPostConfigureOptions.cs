using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationPostConfigureOptions : GrandIdAuthenticationPostConfigureOptions<
        GrandIdBankIdAuthenticationOptions, GrandIdBankIdAuthenticationHandler>
    {
        public GrandIdBankIdAuthenticationPostConfigureOptions(IDataProtectionProvider dataProtection)
            : base(dataProtection)
        {
        }
    }
}

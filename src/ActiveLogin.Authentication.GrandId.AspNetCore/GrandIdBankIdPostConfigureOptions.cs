using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdPostConfigureOptions : GrandIdPostConfigureOptions<GrandIdBankIdOptions, GrandIdBankIdHandler>
    {
        public GrandIdBankIdPostConfigureOptions(IDataProtectionProvider dataProtection)
            : base(dataProtection)
        {
        }
    }
}
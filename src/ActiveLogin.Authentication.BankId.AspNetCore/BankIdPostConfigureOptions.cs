using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal class BankIdPostConfigureOptions : IPostConfigureOptions<BankIdOptions>
{
    private readonly IDataProtectionProvider _dp;

    public BankIdPostConfigureOptions(IDataProtectionProvider dataProtection)
    {
        _dp = dataProtection;
    }

    public void PostConfigure(string name, BankIdOptions options)
    {
        options.DataProtectionProvider ??= _dp;

        if (options.StateDataFormat == null)
        {
            var dataProtector = options.DataProtectionProvider.CreateProtector(
                typeof(BankIdHandler).FullName ?? nameof(BankIdHandler),
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

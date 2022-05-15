using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiAuthResultProtector : BankIdDataStateProtector<BankIdUiResult>, IBankIdUiResultProtector
{
    public BankIdUiAuthResultProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiOrderResultSerializer())
    {
    }
}

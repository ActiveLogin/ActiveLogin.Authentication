using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiResultProtector : BankIdDataStateProtector<BankIdUiResult>, IBankIdUiResultProtector
{
    public BankIdUiResultProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiResultSerializer())
    {
    }
}

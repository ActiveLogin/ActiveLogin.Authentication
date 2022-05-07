using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiOptionsProtector : BankIdDataStateProtector<BankIdUiOptions>, IBankIdUiOptionsProtector
{
    public BankIdUiOptionsProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiOptionsSerializer())
    {
    }
}

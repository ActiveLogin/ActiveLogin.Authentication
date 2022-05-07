using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiOrderRefProtector : BankIdDataStateProtector<BankIdUiOrderRef>, IBankIdUiOrderRefProtector
{
    public BankIdUiOrderRefProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiOrderRefSerializer())
    {
    }
}

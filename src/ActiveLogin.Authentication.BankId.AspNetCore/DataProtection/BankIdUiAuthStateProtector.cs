using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiAuthStateProtector : BankIdDataStateProtector<BankIdUiAuthState>, IBankIdUiAuthStateProtector
{
    public BankIdUiAuthStateProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiAuthStateSerializer())
    {
    }
}

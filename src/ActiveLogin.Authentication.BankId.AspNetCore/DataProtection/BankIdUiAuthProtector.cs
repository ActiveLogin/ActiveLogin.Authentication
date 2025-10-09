using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiAuthProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<Auth.BankIdUiAuthState>(dataProtectionProvider, new BankIdUiAuthStateSerializer()),
    IBankIdDataStateProtector<Auth.BankIdUiAuthState>
{
}

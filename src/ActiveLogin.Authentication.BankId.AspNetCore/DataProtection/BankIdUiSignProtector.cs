using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiSignProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<BankIdUiSignState>(dataProtectionProvider, new BankIdUiSignStateSerializer()),
    IBankIdDataStateProtector<BankIdUiSignState>
{
}

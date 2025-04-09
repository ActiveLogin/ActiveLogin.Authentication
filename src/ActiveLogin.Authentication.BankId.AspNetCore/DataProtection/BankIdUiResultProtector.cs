using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiResultProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<BankIdUiResult>(dataProtectionProvider, new BankIdUiResultSerializer()),
    IBankIdDataStateProtector<BankIdUiResult>
{
}

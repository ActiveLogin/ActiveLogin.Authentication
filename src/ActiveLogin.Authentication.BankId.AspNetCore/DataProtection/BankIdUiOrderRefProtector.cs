using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiOrderRefProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<BankIdUiOrderRef>(dataProtectionProvider, new BankIdUiOrderRefSerializer()),
    IBankIdDataStateProtector<BankIdUiOrderRef>
{
}

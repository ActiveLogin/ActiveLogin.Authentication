using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdQrStartStateProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<BankIdQrStartState>(dataProtectionProvider, new BankIdQrStartStateSerializer()),
    IBankIdDataStateProtector<BankIdQrStartState>
{
}

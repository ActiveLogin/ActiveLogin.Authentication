using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdQrStartStateProtector : BankIdDataStateProtector<BankIdQrStartState>, IBankIdQrStartStateProtector
{
    public BankIdQrStartStateProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdQrStartStateSerializer())
    {
    }
}

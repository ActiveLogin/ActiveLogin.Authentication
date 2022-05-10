using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiSignStateProtector : BankIdDataStateProtector<BankIdUiSignState>, IBankIdUiSignStateProtector
{
    public BankIdUiSignStateProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdUiSignStateSerializer())
    {
    }
}

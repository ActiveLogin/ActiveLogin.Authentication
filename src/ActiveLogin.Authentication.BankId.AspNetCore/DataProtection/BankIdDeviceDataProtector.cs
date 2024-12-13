using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdDeviceDataProtector : BankIdDataStateProtector<DeviceDataState>, IBankIdDeviceDataProtector
{
    public BankIdDeviceDataProtector(IDataProtectionProvider dataProtectionProvider)
        : base(dataProtectionProvider, new BankIdDeviceDataStateSerializer())
    {
    }
}

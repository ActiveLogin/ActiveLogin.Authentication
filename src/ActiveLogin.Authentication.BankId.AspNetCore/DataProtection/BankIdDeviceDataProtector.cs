using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdDeviceDataProtector(
    IDataProtectionProvider dataProtectionProvider
) : BankIdDataStateProtector<DeviceDataState>(dataProtectionProvider, new BankIdDeviceDataStateSerializer()),
    IBankIdDataStateProtector<DeviceDataState>
{
}

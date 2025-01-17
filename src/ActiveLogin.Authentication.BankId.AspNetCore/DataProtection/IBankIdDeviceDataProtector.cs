using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdDeviceDataProtector
{
    string Protect(DeviceDataState deviceDataState);
    DeviceDataState Unprotect(string protectedDeviceDataState);
}

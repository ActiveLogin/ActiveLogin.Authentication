using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IReloadPageOnReturnFromBankIdApp
{
    bool DeviceWillReloadPageOnReturn(BankIdSupportedDevice detectedDevice);
}

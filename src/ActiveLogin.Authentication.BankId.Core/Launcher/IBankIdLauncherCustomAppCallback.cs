using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IBankIdLauncherCustomAppCallback
{
    public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context);
    public Task<string> GetCustomAppReturnUrl(BankIdLauncherCustomAppCallbackContext context);
    public ReloadBehaviourOnReturnFromBankIdApp ReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice) => ReloadBehaviourOnReturnFromBankIdApp.Default;
}

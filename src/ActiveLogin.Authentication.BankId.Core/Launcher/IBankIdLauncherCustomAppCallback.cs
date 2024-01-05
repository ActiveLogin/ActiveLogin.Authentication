namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IBankIdLauncherCustomAppCallback
{
    public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context);
    public Task<BankIdLauncherCustomAppCallbackResult> GetCustomAppCallbackResult(BankIdLauncherCustomAppCallbackContext context);
}

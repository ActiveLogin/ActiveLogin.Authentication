namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IBankIdLauncherCustomAppCallback
{
    public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context);
    public Task<string> GetCustomAppReturnUrl(BankIdLauncherCustomAppCallbackContext context);
}

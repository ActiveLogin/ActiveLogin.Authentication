namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface IBankIdLauncherCustomBrowser
{
    public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context);
    public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context);
}

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLauncherCustomBrowserByContext(
    Func<BankIdLauncherCustomBrowserContext, bool> isApplicable,
    Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> getResult
) : IBankIdLauncherCustomBrowser
{
    public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context)
    {
        return Task.FromResult(isApplicable(context));
    }

    public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context)
    {
        return Task.FromResult(getResult(context));
    }
}

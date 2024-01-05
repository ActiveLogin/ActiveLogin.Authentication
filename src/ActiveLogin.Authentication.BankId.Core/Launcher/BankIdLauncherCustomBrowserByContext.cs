namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLauncherCustomBrowserByContext : IBankIdLauncherCustomBrowser
{
    private readonly Func<BankIdLauncherCustomBrowserContext, bool> _isApplicable;
    private readonly Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> _getResult;

    public BankIdLauncherCustomBrowserByContext(Func<BankIdLauncherCustomBrowserContext, bool> isApplicable, Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> getResult)
    {
        _isApplicable = isApplicable;
        _getResult = getResult;
    }

    public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context)
    {
        var isApplicable = _isApplicable(context);
        return Task.FromResult(isApplicable);
    }

    public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context)
    {
        var result = _getResult(context);
        return Task.FromResult(result);
    }
}

using ActiveLogin.Authentication.BankId.Core.Launcher;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

public class BankIdLauncherUserAgentCustomBrowser : IBankIdLauncherCustomBrowser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Func<string, bool> _isApplicable;
    private readonly Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> _getResult;

    public BankIdLauncherUserAgentCustomBrowser(IHttpContextAccessor httpContextAccessor, Func<string, bool> isApplicable, Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> getResult)
    {
        _httpContextAccessor = httpContextAccessor;
        _isApplicable = isApplicable;
        _getResult = getResult;
    }

    public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context)
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return Task.FromResult(false);
        }

        var isApplicable = _isApplicable(userAgent);
        return Task.FromResult(isApplicable);
    }

    public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context)
    {
        var result = _getResult(context);
        return Task.FromResult(result);
    }
}

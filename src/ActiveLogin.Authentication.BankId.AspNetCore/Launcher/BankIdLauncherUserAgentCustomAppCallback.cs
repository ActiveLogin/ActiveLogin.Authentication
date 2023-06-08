using ActiveLogin.Authentication.BankId.Core.Launcher;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

public class BankIdLauncherUserAgentCustomAppCallback : IBankIdLauncherCustomAppCallback
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Func<string, bool> _isApplicable;
    private readonly Func<BankIdLauncherCustomAppCallbackContext, string> _getReturnUrl;

    public BankIdLauncherUserAgentCustomAppCallback(IHttpContextAccessor httpContextAccessor, Func<string, bool> isApplicable, Func<BankIdLauncherCustomAppCallbackContext, string> getReturnUrl)
    {
        _httpContextAccessor = httpContextAccessor;
        _isApplicable = isApplicable;
        _getReturnUrl = getReturnUrl;
    }

    public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context)
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return Task.FromResult(false);
        }

        var isApplicable = _isApplicable(userAgent);
        return Task.FromResult(isApplicable);
    }

    public Task<string> GetCustomAppReturnUrl(BankIdLauncherCustomAppCallbackContext context)
    {
        var returnUrl = _getReturnUrl(context);
        return Task.FromResult(returnUrl);
    }
}

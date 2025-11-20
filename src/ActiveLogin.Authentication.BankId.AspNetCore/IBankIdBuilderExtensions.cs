using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public static class IBankIdBuilderExtensions
{
    /// <summary>
    /// Adds a custom return url resolver.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [Obsolete("Use AddCustomBrowserByUserAgent that returns a BankIdLauncherCustomBrowserConfig instead.")]
    public static IBankIdBuilder AddCustomBrowserByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, string returnUrl)
    {
        return AddCustomBrowserByUserAgent(builder, isApplicable, context => returnUrl);
    }

    /// <summary>
    /// Adds support for a custom browser (like a third party app).
    /// When only returnUrl is specified, the reload behaviour will fall back to "Never". As we know, only Safari on iOS have the Always behaviour.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="getReturnUrl"></param>
    /// <returns></returns>
    [Obsolete("Use AddCustomBrowserByUserAgent that returns a BankIdLauncherCustomBrowserConfig instead.")]
    public static IBankIdBuilder AddCustomBrowserByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, Func<BankIdLauncherCustomBrowserContext, string> getReturnUrl)
    {
        BankIdLauncherCustomBrowserConfig GetResult(BankIdLauncherCustomBrowserContext context) => new(getReturnUrl(context), BrowserReloadBehaviourOnReturnFromBankIdApp.Never);

        return AddCustomBrowserByUserAgent(builder, isApplicable, GetResult);
    }

    /// <summary>
    /// Adds support for a custom browser (like a third party app).
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="getResult"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomBrowserByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, Func<BankIdLauncherCustomBrowserContext, BankIdLauncherCustomBrowserConfig> getResult)
    {
        builder.Services.AddTransient<IBankIdLauncherCustomBrowser>(x =>
        {
            var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
            return new BankIdLauncherCustomBrowserByUserAgent(httpContextAccessor, isApplicable, getResult);
        });

        return builder;
    }
}

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
    /// When only returnUrl is specified, the reload behaviour will fall back to "Never". As we know, only Safari on iOS have the Always behaviour.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="getReturnUrl"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomAppCallbackByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, Func<BankIdLauncherCustomAppCallbackContext, string> getReturnUrl)
    {
        Func<BankIdLauncherCustomAppCallbackContext, BankIdLauncherCustomAppCallbackResult> getResult =
                (context) => new(getReturnUrl(context), BrowserReloadBehaviourOnReturnFromBankIdApp.Never);

        return AddCustomAppCallbackByUserAgent(builder, isApplicable, getResult);
    }

    /// <summary>
    /// Adds a custom return url resolver.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="getResult"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomAppCallbackByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, Func<BankIdLauncherCustomAppCallbackContext, BankIdLauncherCustomAppCallbackResult> getResult)
    {
        builder.Services.AddTransient<IBankIdLauncherCustomAppCallback>(x =>
        {
            var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
            var customApp = new BankIdLauncherUserAgentCustomAppCallback(httpContextAccessor, isApplicable, getResult);
            return customApp;
        });

        return builder;
    }

    /// <summary>
    /// Adds a custom return url resolver.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isApplicable"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomAppCallbackByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, string returnUrl)
    {
        return AddCustomAppCallbackByUserAgent(builder, isApplicable, context => returnUrl);
    }
}

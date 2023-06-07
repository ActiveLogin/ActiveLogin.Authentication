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
    /// <param name="getReturnUrl"></param>
    /// <returns></returns>
    public static IBankIdBuilder AddCustomAppCallbackByUserAgent(this IBankIdBuilder builder, Func<string, bool> isApplicable, Func<BankIdLauncherCustomAppCallbackContext, string> getReturnUrl)
    {
        builder.Services.AddTransient<IBankIdLauncherCustomAppCallback>(x =>
        {
            var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
            var customApp = new BankIdLauncherUserAgentCustomAppCallback(httpContextAccessor, isApplicable, getReturnUrl);
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
        return AddCustomAppCallbackByUserAgent(builder, isApplicable, s => returnUrl);
    }
}

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderSchemeExtensions
    {
        internal static IBankIdAuthenticationBuilder AddScheme(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callpackPath, Action<BankIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure<BankIdAuthenticationOptions>(authenticationScheme, options =>
            {
                options.CallbackPath = callpackPath;
            });

            builder.AuthenticationBuilder.AddScheme<BankIdAuthenticationOptions, BankIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder)
            => AddCustom(builder, BankIdAuthenticationDefaults.AuthenticationScheme, BankIdAuthenticationDefaults.DisplayName, options => { });

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
            => AddCustom(builder, BankIdAuthenticationDefaults.AuthenticationScheme, BankIdAuthenticationDefaults.DisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
            => AddCustom(builder, authenticationScheme, BankIdAuthenticationDefaults.DisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, BankIdAuthenticationDefaults.CallpackPath, configureOptions);
    }
}
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderSchemeExtensions
    {
        internal static IGrandIdAuthenticationBuilder AddBankIdScheme(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callpackPath, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdBankIdAuthenticationOptions>, GrandIdBankIdAuthenticationPostConfigureOptions>());
            builder.AuthenticationBuilder.Services.Configure<GrandIdBankIdAuthenticationOptions>(authenticationScheme, options =>
            {
                options.CallbackPath = callpackPath;
            });

            builder.AuthenticationBuilder.AddScheme<GrandIdBankIdAuthenticationOptions, GrandIdBankIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }

        public static IGrandIdAuthenticationBuilder AddBankIdSameDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdSameDevice(builder, GrandIdAuthenticationDefaults.BankIdSameDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.BankIdSameDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdSameDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdSameDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.BankIdSameDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdSameDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.BankIdSameDeviceCallpackPath, configureOptions);


        public static IGrandIdAuthenticationBuilder AddBankIdOtherDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdOtherDevice(builder, GrandIdAuthenticationDefaults.BankIdOtherDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.BankIdOtherDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdOtherDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdOtherDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.BankIdOtherDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdOtherDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.BankIdOtherDeviceCallpackPath, configureOptions);


        public static IGrandIdAuthenticationBuilder AddBankIdChooseDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdChooseDevice(builder, GrandIdAuthenticationDefaults.BankIdChooseDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.BankIdChooseDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdChooseDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdChooseDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.BankIdChooseDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddBankIdChooseDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdAuthenticationOptions> configureOptions)
            => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.BankIdChooseDeviceCallpackPath, configureOptions);
    }
}
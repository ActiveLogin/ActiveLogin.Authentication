using System;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderSchemeExtensions
    {
        internal static IBankIdAuthenticationBuilder AddScheme(this IBankIdAuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<BankIdAuthenticationOptions> configureOptions,
            Action<BankIdAuthenticationOptions> defaultConfigureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(authenticationScheme, defaultConfigureOptions);

            builder.AuthenticationBuilder.AddScheme<BankIdAuthenticationOptions, BankIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }


        /// <summary>
        ///     Configures options that will apply to all BankID schemes.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IBankIdAuthenticationBuilder Configure(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.ConfigureAll(configureOptions);
            return builder;
        }

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder)
        {
            return AddSameDevice(builder, BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, options => { });
        }

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddSameDevice(builder, BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);
        }

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddSameDevice(builder, authenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);
        }

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
            {
                options.CallbackPath = BankIdAuthenticationDefaults.SameDeviceCallbackPath;
                options.BankIdAutoLaunch = true;
                options.BankIdAllowChangingPersonalIdentityNumber = false;
            });
        }


        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder)
        {
            return AddOtherDevice(builder, BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, options => { });
        }

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddOtherDevice(builder, BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);
        }

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddOtherDevice(builder, authenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);
        }

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
            {
                options.CallbackPath = BankIdAuthenticationDefaults.OtherDeviceCallbackPath;
                options.BankIdAutoLaunch = false;
                options.BankIdAllowChangingPersonalIdentityNumber = true;
            });
        }
    }
}

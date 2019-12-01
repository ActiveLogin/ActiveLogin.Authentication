using System;
using ActiveLogin.Authentication.BankId.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderSchemeExtensions
    {
        internal static IBankIdBuilder AddScheme(this IBankIdBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions, Action<BankIdOptions> defaultConfigureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(authenticationScheme, defaultConfigureOptions);

            builder.AuthenticationBuilder.AddScheme<BankIdOptions, BankIdHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }


        /// <summary>
        /// Configures options that will apply to all BankID schemes.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IBankIdBuilder Configure(this IBankIdBuilder builder, Action<BankIdOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.ConfigureAll(configureOptions);
            return builder;
        }

        public static IBankIdBuilder AddSameDevice(this IBankIdBuilder builder)
            => AddSameDevice(builder, BankIdDefaults.SameDeviceAuthenticationScheme, BankIdDefaults.SameDeviceDisplayName, options => { });

        public static IBankIdBuilder AddSameDevice(this IBankIdBuilder builder, Action<BankIdOptions> configureOptions)
            => AddSameDevice(builder, BankIdDefaults.SameDeviceAuthenticationScheme, BankIdDefaults.SameDeviceDisplayName, configureOptions);

        public static IBankIdBuilder AddSameDevice(this IBankIdBuilder builder, string authenticationScheme, Action<BankIdOptions> configureOptions)
            => AddSameDevice(builder, authenticationScheme, BankIdDefaults.SameDeviceDisplayName, configureOptions);

        public static IBankIdBuilder AddSameDevice(this IBankIdBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
                {
                    options.CallbackPath = BankIdDefaults.SameDeviceCallbackPath;
                    options.BankIdAutoLaunch = true;
                    options.BankIdAllowChangingPersonalIdentityNumber = false;
                });


        public static IBankIdBuilder AddOtherDevice(this IBankIdBuilder builder)
            => AddOtherDevice(builder, BankIdDefaults.OtherDeviceAuthenticationScheme, BankIdDefaults.OtherDeviceDisplayName, options => { });

        public static IBankIdBuilder AddOtherDevice(this IBankIdBuilder builder, Action<BankIdOptions> configureOptions)
            => AddOtherDevice(builder, BankIdDefaults.OtherDeviceAuthenticationScheme, BankIdDefaults.OtherDeviceDisplayName, configureOptions);

        public static IBankIdBuilder AddOtherDevice(this IBankIdBuilder builder, string authenticationScheme, Action<BankIdOptions> configureOptions)
            => AddOtherDevice(builder, authenticationScheme, BankIdDefaults.OtherDeviceDisplayName, configureOptions);

        public static IBankIdBuilder AddOtherDevice(this IBankIdBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
            {
                options.CallbackPath = BankIdDefaults.OtherDeviceCallbackPath;
                options.BankIdAutoLaunch = false;
                options.BankIdAllowChangingPersonalIdentityNumber = true;
                options.BankIdUseQrCode = true;
            });
    }
}

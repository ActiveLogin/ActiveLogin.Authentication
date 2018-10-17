﻿using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderSchemeExtensions
    {
        internal static IBankIdAuthenticationBuilder AddScheme(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callpackPath, Action<BankIdAuthenticationOptions> configureOptions)
        {
            builder.AddScheme(authenticationScheme, displayName, configureOptions, options =>
                {
                    options.CallbackPath = callpackPath;
                });

            return builder;
        }

        internal static IBankIdAuthenticationBuilder AddScheme(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions, Action<BankIdAuthenticationOptions> defaultConfigureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(authenticationScheme, defaultConfigureOptions);

            builder.AuthenticationBuilder.AddScheme<BankIdAuthenticationOptions, BankIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }


        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder)
            => AddCustom(builder, BankIdAuthenticationDefaults.CustomAuthenticationScheme, BankIdAuthenticationDefaults.CustomDisplayName, options => { });

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
            => AddCustom(builder, BankIdAuthenticationDefaults.CustomAuthenticationScheme, BankIdAuthenticationDefaults.CustomDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
            => AddCustom(builder, authenticationScheme, BankIdAuthenticationDefaults.CustomDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddCustom(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, BankIdAuthenticationDefaults.CustomCallpackPath, configureOptions);



        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder)
            => AddSameDevice(builder, BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, options => { });

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
            => AddSameDevice(builder, BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
            => AddSameDevice(builder, authenticationScheme, BankIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddSameDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
                {
                    options.CallbackPath = BankIdAuthenticationDefaults.SameDeviceCallpackPath;
                    options.BankIdAutoLaunch = true;
                    options.BankIdAllowChangingPersonalIdentityNumber = false;
                });


        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder)
            => AddOtherDevice(builder, BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, options => { });

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, Action<BankIdAuthenticationOptions> configureOptions)
            => AddOtherDevice(builder, BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
            => AddOtherDevice(builder, authenticationScheme, BankIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);

        public static IBankIdAuthenticationBuilder AddOtherDevice(this IBankIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
            {
                options.CallbackPath = BankIdAuthenticationDefaults.OtherDeviceCallpackPath;
                options.BankIdAutoLaunch = false;
                options.BankIdAllowChangingPersonalIdentityNumber = true;
            });
    }
}
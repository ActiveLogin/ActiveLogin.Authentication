using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderSchemeExtensions
    {
        internal static IGrandIdAuthenticationBuilder AddScheme(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callbackPath, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, options =>
            {
                options.CallbackPath = callbackPath;
                configureOptions(options);
            });

        internal static IGrandIdAuthenticationBuilder AddScheme(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callbackPath, string authenticateServiceKey)
            => AddScheme(builder, authenticationScheme, displayName, options =>
            {
                options.CallbackPath = callbackPath;
                options.AuthenticateServiceKey = authenticateServiceKey;
            });

        internal static IGrandIdAuthenticationBuilder AddScheme(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.AddScheme<GrandIdAuthenticationOptions, GrandIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }


        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.SameDeviceCallpackPath, configureOptions);

        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, GrandIdAuthenticationDefaults.SameDeviceAuthenticationScheme, displayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, string displayName, string authenticateServiceKey)
            => AddScheme(builder, GrandIdAuthenticationDefaults.SameDeviceAuthenticationScheme, displayName, GrandIdAuthenticationDefaults.SameDeviceCallpackPath, authenticateServiceKey);


        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.OtherDeviceCallpackPath, configureOptions);

        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, GrandIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, displayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, string displayName, string authenticateServiceKey)
            => AddScheme(builder, GrandIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, displayName, GrandIdAuthenticationDefaults.OtherDeviceCallpackPath, authenticateServiceKey);


        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.ChooseDeviceCallpackPath, configureOptions);

        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, GrandIdAuthenticationDefaults.ChooseDeviceAuthenticationScheme, displayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, string displayName, string authenticateServiceKey)
            => AddScheme(builder, GrandIdAuthenticationDefaults.ChooseDeviceAuthenticationScheme, displayName, GrandIdAuthenticationDefaults.ChooseDeviceCallpackPath, authenticateServiceKey);

    }
}
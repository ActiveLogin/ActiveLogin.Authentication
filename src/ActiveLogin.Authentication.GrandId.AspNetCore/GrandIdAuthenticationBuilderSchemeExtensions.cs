using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderSchemeExtensions
    {
        internal static IGrandIdAuthenticationBuilder AddScheme(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, PathString callpackPath, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure<GrandIdAuthenticationOptions>(authenticationScheme, options =>
            {
                options.CallbackPath = callpackPath;
            });

            builder.AuthenticationBuilder.AddScheme<GrandIdAuthenticationOptions, GrandIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }

        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddSameDevice(builder, GrandIdAuthenticationDefaults.SameDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddSameDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.SameDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddSameDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.SameDeviceCallpackPath, configureOptions);


        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddOtherDevice(builder, GrandIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddOtherDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.OtherDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddOtherDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.OtherDeviceCallpackPath, configureOptions);


        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddChooseDevice(builder, GrandIdAuthenticationDefaults.ChooseDeviceAuthenticationScheme, GrandIdAuthenticationDefaults.ChooseDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddChooseDevice(builder, authenticationScheme, GrandIdAuthenticationDefaults.ChooseDeviceDisplayName, configureOptions);

        public static IGrandIdAuthenticationBuilder AddChooseDevice(this IGrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
            => AddScheme(builder, authenticationScheme, displayName, GrandIdAuthenticationDefaults.ChooseDeviceCallpackPath, configureOptions);
    }
}
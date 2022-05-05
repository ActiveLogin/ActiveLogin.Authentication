using System;
using ActiveLogin.Authentication.GrandId.AspNetCore;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class GrandIdBuilderSchemeExtensions
{
    internal static IGrandIdBuilder AddBankIdScheme(this IGrandIdBuilder builder, string authenticationScheme, string displayName, PathString callpackPath, GrandIdBankIdMode mode, Action<GrandIdBankIdOptions> configureOptions)
    {
        builder.AuthenticationBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdBankIdOptions>, GrandIdBankIdPostConfigureOptions>());
        builder.AuthenticationBuilder.Services.Configure<GrandIdBankIdOptions>(authenticationScheme, options =>
        {
            options.CallbackPath = callpackPath;
            options.GrandIdBankIdMode = mode;
        });

        builder.AuthenticationBuilder.AddScheme<GrandIdBankIdOptions, GrandIdBankIdHandler>(
            authenticationScheme,
            displayName,
            configureOptions
        );

        return builder;
    }

    /// <summary>
    /// Configures options that will apply to all GrandID BankID schemes.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IGrandIdBuilder ConfigureBankId(this IGrandIdBuilder builder, Action<GrandIdBankIdOptions> configureOptions)
    {
        builder.AuthenticationBuilder.Services.ConfigureAll(configureOptions);
        return builder;
    }


    public static IGrandIdBuilder AddBankIdSameDevice(this IGrandIdBuilder builder)
        => AddBankIdSameDevice(builder, options => { });

    public static IGrandIdBuilder AddBankIdSameDevice(this IGrandIdBuilder builder, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdSameDevice(builder, GrandIdDefaults.BankIdSameDeviceAuthenticationScheme, GrandIdDefaults.BankIdSameDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdSameDevice(this IGrandIdBuilder builder, string authenticationScheme, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdSameDevice(builder, authenticationScheme, GrandIdDefaults.BankIdSameDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdSameDevice(this IGrandIdBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdDefaults.BankIdSameDeviceCallbackPath, GrandIdBankIdMode.SameDevice, configureOptions);


    public static IGrandIdBuilder AddBankIdOtherDevice(this IGrandIdBuilder builder)
        => AddBankIdOtherDevice(builder, options => { });

    public static IGrandIdBuilder AddBankIdOtherDevice(this IGrandIdBuilder builder, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdOtherDevice(builder, GrandIdDefaults.BankIdOtherDeviceAuthenticationScheme, GrandIdDefaults.BankIdOtherDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdOtherDevice(this IGrandIdBuilder builder, string authenticationScheme, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdOtherDevice(builder, authenticationScheme, GrandIdDefaults.BankIdOtherDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdOtherDevice(this IGrandIdBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdDefaults.BankIdOtherDeviceCallbackPath, GrandIdBankIdMode.OtherDevice, configureOptions);


    public static IGrandIdBuilder AddBankIdChooseDevice(this IGrandIdBuilder builder)
        => AddBankIdChooseDevice(builder, options => { });

    public static IGrandIdBuilder AddBankIdChooseDevice(this IGrandIdBuilder builder, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdChooseDevice(builder, GrandIdDefaults.BankIdChooseDeviceAuthenticationScheme, GrandIdDefaults.BankIdChooseDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdChooseDevice(this IGrandIdBuilder builder, string authenticationScheme, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdChooseDevice(builder, authenticationScheme, GrandIdDefaults.BankIdChooseDeviceDisplayName, configureOptions);

    public static IGrandIdBuilder AddBankIdChooseDevice(this IGrandIdBuilder builder, string authenticationScheme, string displayName, Action<GrandIdBankIdOptions> configureOptions)
        => AddBankIdScheme(builder, authenticationScheme, displayName, GrandIdDefaults.BankIdChooseDeviceCallbackPath, GrandIdBankIdMode.ChooseDevice, configureOptions);
}
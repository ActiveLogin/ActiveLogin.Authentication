using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Requirements;
using ActiveLogin.Authentication.BankId.Core.UserData;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public static class IBankIdAuthBuilderExtensions
{
    /// <summary>
    /// Add a custom claims transformer.
    /// </summary>
    /// <typeparam name="TBankIdClaimsTransformerImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddClaimsTransformer<TBankIdClaimsTransformerImplementation>(this IBankIdAuthBuilder builder) where TBankIdClaimsTransformerImplementation : class, IBankIdClaimsTransformer
    {
        builder.Services.AddTransient<IBankIdClaimsTransformer, TBankIdClaimsTransformerImplementation>();

        return builder;
    }


    /// <summary>
    /// Set what user data to supply to the auth request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authUserData"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder UseAuthRequestUserData(this IBankIdAuthBuilder builder, BankIdAuthUserData authUserData)
    {
        builder.Services.AddTransient<IBankIdAuthRequestUserDataResolver>(x => new BankIdAuthRequestStaticUserDataResolver(authUserData));

        return builder;
    }

    /// <summary>
    /// Set what user data to supply to the auth request.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authUserData"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder UseAuthRequestUserData(this IBankIdAuthBuilder builder, Action<BankIdAuthUserData> authUserData)
    {
        var authUserDataResult = new BankIdAuthUserData();
        authUserData(authUserDataResult);
        UseAuthRequestUserData(builder, authUserDataResult);

        return builder;
    }

    /// <summary>
    /// Use a custom user data resolver.
    /// </summary>
    /// <typeparam name="TBankIdAuthRequestUserDataResolverImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder UseAuthRequestUserDataResolver<TBankIdAuthRequestUserDataResolverImplementation>(this IBankIdAuthBuilder builder) where TBankIdAuthRequestUserDataResolverImplementation : class, IBankIdAuthRequestUserDataResolver
    {
        builder.Services.AddTransient<IBankIdAuthRequestUserDataResolver, TBankIdAuthRequestUserDataResolverImplementation>();

        return builder;
    }

    /// <summary>
    /// Use a custom requirements resolver.
    /// </summary>
    /// <typeparam name="TBankIdAuthRequestRequirementsResolverImplementation"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder UseAuthRequestRequirementsResolver<TBankIdAuthRequestRequirementsResolverImplementation>(this IBankIdAuthBuilder builder) where TBankIdAuthRequestRequirementsResolverImplementation : class, IBankIdAuthRequestRequirementsResolver
    {
        builder.Services.AddTransient<IBankIdAuthRequestRequirementsResolver, TBankIdAuthRequestRequirementsResolverImplementation>();

        return builder;
    }


    /// <summary>
    /// Configures options that will apply to all BankID schemes.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder Configure(this IBankIdAuthBuilder builder, Action<BankIdAuthOptions> configureOptions)
    {
        builder.Services.ConfigureAll(configureOptions);
        return builder;
    }

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder)
        => AddSameDevice(builder, BankIdAuthDefaults.SameDeviceAuthenticationScheme, BankIdAuthDefaults.SameDeviceDisplayName, options => { });

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, Action<BankIdAuthOptions> configureOptions)
        => AddSameDevice(builder, BankIdAuthDefaults.SameDeviceAuthenticationScheme, BankIdAuthDefaults.SameDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, string authenticationScheme, Action<BankIdAuthOptions> configureOptions)
        => AddSameDevice(builder, authenticationScheme, BankIdAuthDefaults.SameDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthOptions> configureOptions)
        => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
        {
            options.CallbackPath = BankIdAuthDefaults.SameDeviceCallbackPath;
            options.BankIdSameDevice = true;
        });


    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder)
        => AddOtherDevice(builder, BankIdAuthDefaults.OtherDeviceAuthenticationScheme, BankIdAuthDefaults.OtherDeviceDisplayName, options => { });

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, Action<BankIdAuthOptions> configureOptions)
        => AddOtherDevice(builder, BankIdAuthDefaults.OtherDeviceAuthenticationScheme, BankIdAuthDefaults.OtherDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, string authenticationScheme, Action<BankIdAuthOptions> configureOptions)
        => AddOtherDevice(builder, authenticationScheme, BankIdAuthDefaults.OtherDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthOptions> configureOptions)
        => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
        {
            options.CallbackPath = BankIdAuthDefaults.OtherDeviceCallbackPath;
            options.BankIdSameDevice = false;
        });


    internal static IBankIdAuthBuilder AddScheme(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthOptions> configureOptions, Action<BankIdAuthOptions> defaultConfigureOptions)
    {
        builder.Services.Configure(authenticationScheme, defaultConfigureOptions);

        builder.AuthenticationBuilder.AddScheme<BankIdAuthOptions, BankIdAuthHandler>(
            authenticationScheme,
            displayName,
            configureOptions
        );

        return builder;
    }

    public static IBankIdAuthBuilder AddStateStorage<T>(this IBankIdAuthBuilder builder, T storage)
        where T : class, IStateStorage
    {
        builder.Services.AddSingleton<IStateStorage>(storage);
        return builder;
    }

    public static IBankIdAuthBuilder AddStateStorage<T>(this IBankIdAuthBuilder builder)
        where T : class, IStateStorage
    {
        builder.Services.AddSingleton<IStateStorage, T>();
        return builder;
    }

    public static IBankIdAuthBuilder AddStateStorage(this IBankIdAuthBuilder builder, Func<IServiceProvider, IStateStorage> factory)
    {
        builder.Services.AddSingleton(factory);
        return builder;
    }
}

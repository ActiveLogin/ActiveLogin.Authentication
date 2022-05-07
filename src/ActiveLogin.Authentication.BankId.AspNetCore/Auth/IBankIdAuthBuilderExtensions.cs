using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

namespace Microsoft.Extensions.DependencyInjection;
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
}

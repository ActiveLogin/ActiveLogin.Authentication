using ActiveLogin.Authentication.BankId.AspNetCore;
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
    public static IBankIdAuthBuilder Configure(this IBankIdAuthBuilder builder, Action<BankIdOptions> configureOptions)
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
        => AddSameDevice(builder, BankIdDefaults.SameDeviceAuthenticationScheme, BankIdDefaults.SameDeviceDisplayName, options => { });

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, Action<BankIdOptions> configureOptions)
        => AddSameDevice(builder, BankIdDefaults.SameDeviceAuthenticationScheme, BankIdDefaults.SameDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, string authenticationScheme, Action<BankIdOptions> configureOptions)
        => AddSameDevice(builder, authenticationScheme, BankIdDefaults.SameDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdAuthBuilder AddSameDevice(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions)
        => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
        {
            options.CallbackPath = BankIdDefaults.SameDeviceCallbackPath;
            options.BankIdSameDevice = true;
        });


    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder)
        => AddOtherDevice(builder, BankIdDefaults.OtherDeviceAuthenticationScheme, BankIdDefaults.OtherDeviceDisplayName, options => { });

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, Action<BankIdOptions> configureOptions)
        => AddOtherDevice(builder, BankIdDefaults.OtherDeviceAuthenticationScheme, BankIdDefaults.OtherDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, string authenticationScheme, Action<BankIdOptions> configureOptions)
        => AddOtherDevice(builder, authenticationScheme, BankIdDefaults.OtherDeviceDisplayName, configureOptions);

    /// <summary>
    /// Add scheme for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdAuthBuilder AddOtherDevice(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions)
        => AddScheme(builder, authenticationScheme, displayName, configureOptions, options =>
        {
            options.CallbackPath = BankIdDefaults.OtherDeviceCallbackPath;
            options.BankIdSameDevice = false;
        });


    internal static IBankIdAuthBuilder AddScheme(this IBankIdAuthBuilder builder, string authenticationScheme, string displayName, Action<BankIdOptions> configureOptions, Action<BankIdOptions> defaultConfigureOptions)
    {
        builder.Services.Configure(authenticationScheme, defaultConfigureOptions);

        builder.AuthenticationBuilder.AddScheme<BankIdOptions, BankIdHandler>(
            authenticationScheme,
            displayName,
            configureOptions
        );

        return builder;
    }
}

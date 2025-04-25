using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public static class IBankIdPaymentBuilderExtensions
{
    /// <summary>
    /// Configures options that will apply to all BankID config.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IBankIdPaymentBuilder Configure(this IBankIdPaymentBuilder builder, Action<BankIdPaymentOptions> configureOptions)
    {
        builder.Services.ConfigureAll(configureOptions);
        return builder;
    }

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdPaymentBuilder AddSameDevice(this IBankIdPaymentBuilder builder)
        => AddSameDevice(builder, BankIdPaymentDefaults.SameDeviceConfigKey, BankIdPaymentDefaults.SameDeviceConfigDisplayName, options => { });

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdPaymentBuilder AddSameDevice(this IBankIdPaymentBuilder builder, Action<BankIdPaymentOptions> configureOptions)
        => AddSameDevice(builder, BankIdPaymentDefaults.SameDeviceConfigKey, BankIdPaymentDefaults.SameDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdPaymentBuilder AddSameDevice(this IBankIdPaymentBuilder builder, string configKey, Action<BankIdPaymentOptions> configureOptions)
        => AddSameDevice(builder, configKey, BankIdPaymentDefaults.SameDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdPaymentBuilder AddSameDevice(this IBankIdPaymentBuilder builder, string configKey, string displayName, Action<BankIdPaymentOptions> configureOptions)
        => AddConfig(builder, configKey, displayName, configureOptions, options =>
        {
            options.BankIdSameDevice = true;
        });


    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    public static IBankIdPaymentBuilder AddOtherDevice(this IBankIdPaymentBuilder builder)
        => AddOtherDevice(builder, BankIdPaymentDefaults.OtherDeviceConfigKey, BankIdPaymentDefaults.OtherDeviceConfigDisplayName, options => { });

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdPaymentBuilder AddOtherDevice(this IBankIdPaymentBuilder builder, Action<BankIdPaymentOptions> configureOptions)
        => AddOtherDevice(builder, BankIdPaymentDefaults.OtherDeviceConfigKey, BankIdPaymentDefaults.OtherDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdPaymentBuilder AddOtherDevice(this IBankIdPaymentBuilder builder, string configKey, Action<BankIdPaymentOptions> configureOptions)
        => AddOtherDevice(builder, configKey, BankIdPaymentDefaults.OtherDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdPaymentBuilder AddOtherDevice(this IBankIdPaymentBuilder builder, string configKey, string displayName, Action<BankIdPaymentOptions> configureOptions)
        => AddConfig(builder, configKey, displayName, configureOptions, options =>
        {
            options.BankIdSameDevice = false;
        });


    internal static IBankIdPaymentBuilder AddConfig(this IBankIdPaymentBuilder builder, string configKey, string displayName, Action<BankIdPaymentOptions> configureOptions, Action<BankIdPaymentOptions> defaultConfigureOptions)
    {
        builder.Services.Configure(configKey, defaultConfigureOptions);

        builder.AddConfig(
            configKey,
            displayName,
            configureOptions
        );

        return builder;
    }
}

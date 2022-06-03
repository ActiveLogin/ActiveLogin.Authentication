using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public static class IBankIdSignBuilderExtensions
{
    /// <summary>
    /// Configures options that will apply to all BankID config.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IBankIdSignBuilder Configure(this IBankIdSignBuilder builder, Action<BankIdSignOptions> configureOptions)
    {
        builder.Services.ConfigureAll(configureOptions);
        return builder;
    }

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdSignBuilder AddSameDevice(this IBankIdSignBuilder builder)
        => AddSameDevice(builder, BankIdSignDefaults.SameDeviceConfigKey, BankIdSignDefaults.SameDeviceConfigDisplayName, options => { });

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdSignBuilder AddSameDevice(this IBankIdSignBuilder builder, Action<BankIdSignOptions> configureOptions)
        => AddSameDevice(builder, BankIdSignDefaults.SameDeviceConfigKey, BankIdSignDefaults.SameDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdSignBuilder AddSameDevice(this IBankIdSignBuilder builder, string configKey, Action<BankIdSignOptions> configureOptions)
        => AddSameDevice(builder, configKey, BankIdSignDefaults.SameDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Same Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    /// <returns></returns>
    public static IBankIdSignBuilder AddSameDevice(this IBankIdSignBuilder builder, string configKey, string displayName, Action<BankIdSignOptions> configureOptions)
        => AddConfig(builder, configKey, displayName, configureOptions, options =>
        {
            options.BankIdSameDevice = true;
        });


    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    public static IBankIdSignBuilder AddOtherDevice(this IBankIdSignBuilder builder)
        => AddOtherDevice(builder, BankIdSignDefaults.OtherDeviceConfigKey, BankIdSignDefaults.OtherDeviceConfigDisplayName, options => { });

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdSignBuilder AddOtherDevice(this IBankIdSignBuilder builder, Action<BankIdSignOptions> configureOptions)
        => AddOtherDevice(builder, BankIdSignDefaults.OtherDeviceConfigKey, BankIdSignDefaults.OtherDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdSignBuilder AddOtherDevice(this IBankIdSignBuilder builder, string configKey, Action<BankIdSignOptions> configureOptions)
        => AddOtherDevice(builder, configKey, BankIdSignDefaults.OtherDeviceConfigDisplayName, configureOptions);

    /// <summary>
    /// Add config for BankID on Other Device.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configKey">Scheme name</param>
    /// <param name="displayName">Scheme display name</param>
    /// <param name="configureOptions">BankId Options</param>
    public static IBankIdSignBuilder AddOtherDevice(this IBankIdSignBuilder builder, string configKey, string displayName, Action<BankIdSignOptions> configureOptions)
        => AddConfig(builder, configKey, displayName, configureOptions, options =>
        {
            options.BankIdSameDevice = false;
        });


    internal static IBankIdSignBuilder AddConfig(this IBankIdSignBuilder builder, string configKey, string displayName, Action<BankIdSignOptions> configureOptions, Action<BankIdSignOptions> defaultConfigureOptions)
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

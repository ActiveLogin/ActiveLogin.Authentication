using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

internal class BankIdPaymentBuilder : IBankIdPaymentBuilder
{
    private readonly IBankIdPaymentConfigurationProvider _bankIdPaymentConfigurationProvider;

    public IServiceCollection Services { get; }

    public BankIdPaymentBuilder(IServiceCollection services, IBankIdPaymentConfigurationProvider bankIdPaymentConfigurationProvider)
    {
        _bankIdPaymentConfigurationProvider = bankIdPaymentConfigurationProvider;
        Services = services;
    }

    public void AddConfig(string configKey, string? displayName = null, Action<BankIdPaymentOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            Services.Configure(configKey, configureOptions);
        }

        _bankIdPaymentConfigurationProvider.AddConfiguration(configKey, displayName);
    }
}

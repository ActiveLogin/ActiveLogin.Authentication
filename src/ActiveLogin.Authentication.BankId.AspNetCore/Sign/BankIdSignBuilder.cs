using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

internal class BankIdSignBuilder : IBankIdSignBuilder
{
    private readonly IBankIdSignConfigurationProvider _bankIdSignConfigurationProvider;

    public IServiceCollection Services { get; }

    public BankIdSignBuilder(IServiceCollection services, IBankIdSignConfigurationProvider bankIdSignConfigurationProvider)
    {
        _bankIdSignConfigurationProvider = bankIdSignConfigurationProvider;
        Services = services;
    }

    public void AddConfig(string configKey, string? displayName = null, Action<BankIdSignOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            Services.Configure(configKey, configureOptions);
        }

        _bankIdSignConfigurationProvider.AddConfiguration(configKey, displayName);
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

internal class BankIdSignBuilder : IBankIdSignBuilder
{
    public IServiceCollection Services { get; }

    public BankIdSignBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IBankIdSignBuilder AddConfig(string configKey, string displayName, Action<BankIdSignOptions> configureOptions)
    {
        throw new NotImplementedException();
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public interface IBankIdSignBuilder
{
    IServiceCollection Services { get; }

    IBankIdSignBuilder AddConfig(string configKey, string displayName, Action<BankIdSignOptions> configureOptions);
}

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public interface IBankIdSignBuilder
{
    IServiceCollection Services { get; }
    void AddConfig(string configKey, string? displayName = null, Action<BankIdSignOptions>? configureOptions = null);
}

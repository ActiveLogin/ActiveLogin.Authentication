using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public interface IBankIdPaymentBuilder
{
    IServiceCollection Services { get; }
    void AddConfig(string configKey, string? displayName = null, Action<BankIdPaymentOptions>? configureOptions = null);
}

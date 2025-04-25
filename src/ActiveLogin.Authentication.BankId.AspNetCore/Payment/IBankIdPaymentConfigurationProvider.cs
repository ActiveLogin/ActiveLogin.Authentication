namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public interface IBankIdPaymentConfigurationProvider
{
    void AddConfiguration(string key, string? displayName = null);

    Task<List<BankIdPaymentConfiguration>> GetAllConfigurationsAsync();
}

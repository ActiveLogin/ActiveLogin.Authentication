namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

internal class BankIdPaymentConfigurationProvider : IBankIdPaymentConfigurationProvider
{
    private readonly Dictionary<string, BankIdPaymentConfiguration> _configurations = new();

    public Task<List<BankIdPaymentConfiguration>> GetAllConfigurationsAsync()
    {
        return Task.FromResult(_configurations.Values.ToList());
    }

    public void AddConfiguration(string key, string? displayName = null)
    {
        if (!_configurations.TryAdd(key, new BankIdPaymentConfiguration(key, displayName)))
        {
            throw new Exception($"A payment configuration with the key \"{key}\" already exists");
        }
    }
}

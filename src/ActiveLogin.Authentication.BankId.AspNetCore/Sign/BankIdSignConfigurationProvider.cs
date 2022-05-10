namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

internal class BankIdSignConfigurationProvider : IBankIdSignConfigurationProvider
{
    private readonly Dictionary<string, BankIdSignConfiguration> _configurations = new();

    public Task<List<BankIdSignConfiguration>> GetAllConfigurationsAsync()
    {
        return Task.FromResult(_configurations.Values.ToList());
    }

    public void AddConfiguration(string key, string? displayName = null)
    {
        if (!_configurations.TryAdd(key, new BankIdSignConfiguration(key, displayName)))
        {
            throw new Exception($"A sign configuration with the key \"{key}\" already exists");
        }
    }
}

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public interface IBankIdSignConfigurationProvider
{
    void AddConfiguration(string key, string? displayName = null);

    Task<List<BankIdSignConfiguration>> GetAllConfigurationsAsync();
}

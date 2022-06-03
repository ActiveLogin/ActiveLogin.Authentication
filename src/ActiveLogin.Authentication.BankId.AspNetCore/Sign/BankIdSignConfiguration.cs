namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignConfiguration
{
    public BankIdSignConfiguration(string key, string? displayName = null)
    {
        Key = key;
        DisplayName = displayName;
    }

    public string Key { get; }
    public string? DisplayName { get; } }

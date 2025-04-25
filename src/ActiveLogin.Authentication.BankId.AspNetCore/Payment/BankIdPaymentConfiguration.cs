namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdPaymentConfiguration
{
    public BankIdPaymentConfiguration(string key, string? displayName = null)
    {
        Key = key;
        DisplayName = displayName;
    }

    public string Key { get; }
    public string? DisplayName { get; } }

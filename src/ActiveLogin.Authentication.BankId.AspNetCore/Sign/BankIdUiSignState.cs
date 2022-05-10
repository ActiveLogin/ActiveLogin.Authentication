namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdUiSignState
{
    public BankIdUiSignState(string configKey, BankIdSignProperties bankIdSignProperties)
    {
        ConfigKey = configKey;
        BankIdSignProperties = bankIdSignProperties;
    }

    public string ConfigKey { get; set; }
    public BankIdSignProperties BankIdSignProperties { get; set; }
}

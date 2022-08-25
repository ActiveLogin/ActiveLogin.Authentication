using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdUiSignState : BankIdUiState
{
    public override BankIdStateType Type => BankIdStateType.Sign;

    public BankIdUiSignState(string configKey, BankIdSignProperties bankIdSignProperties)
    {
        ConfigKey = configKey;
        BankIdSignProperties = bankIdSignProperties;
    }

    public string ConfigKey { get; set; }
    public BankIdSignProperties BankIdSignProperties { get; set; }
}

using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdUiSignState(string configKey, BankIdSignProperties bankIdSignProperties) : BankIdUiState
{
    public override BankIdStateType Type => BankIdStateType.Sign;

    public string ConfigKey { get; set; } = configKey;
    public BankIdSignProperties BankIdSignProperties { get; set; } = bankIdSignProperties;
}

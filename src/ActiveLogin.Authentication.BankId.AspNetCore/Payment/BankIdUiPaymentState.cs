using ActiveLogin.Authentication.BankId.AspNetCore.Models;
namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdUiPaymentState : BankIdUiState
{
    public override BankIdStateType Type => BankIdStateType.Payment;

    public BankIdUiPaymentState(string configKey, BankIdPaymentProperties bankIdPaymentProperties)
    {
        ConfigKey = configKey;
        BankIdPaymentProperties = bankIdPaymentProperties;
    }

    public string ConfigKey { get; set; }
    public BankIdPaymentProperties BankIdPaymentProperties { get; set; }
}

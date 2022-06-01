namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiAuthViewModel : BankIdUiViewModel
{
    public override string Type => "Auth";

    public BankIdUiAuthViewModel(BankIdUiScriptConfiguration bankUiScriptConfiguration, BankIdUiScriptInitState bankIdUiScriptInitState)
        : base(bankUiScriptConfiguration, bankIdUiScriptInitState)
    {
    }
}

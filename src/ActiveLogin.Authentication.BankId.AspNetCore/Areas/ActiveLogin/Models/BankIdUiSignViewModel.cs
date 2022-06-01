namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiSignViewModel : BankIdUiViewModel
{
    public override string Type => "Sign";

    public BankIdUiSignViewModel(BankIdUiScriptConfiguration bankUiScriptConfiguration, BankIdUiScriptInitState bankIdUiScriptInitState, BankIdUiSignData signData)
        : base(bankUiScriptConfiguration, bankIdUiScriptInitState, signData)
    {
    }
}

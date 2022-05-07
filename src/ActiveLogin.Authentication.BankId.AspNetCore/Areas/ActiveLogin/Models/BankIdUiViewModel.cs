namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiViewModel
{
    internal BankIdUiViewModel(BankIdUiScriptConfiguration bankUiScriptConfiguration, BankIdUiScriptInitState bankIdUiScriptInitState)
    {
        BankUiScriptConfiguration = bankUiScriptConfiguration;
        BankIdUiScriptInitState = bankIdUiScriptInitState;
    }

    public BankIdUiScriptConfiguration BankUiScriptConfiguration { get; }

    public BankIdUiScriptInitState BankIdUiScriptInitState { get; }
}

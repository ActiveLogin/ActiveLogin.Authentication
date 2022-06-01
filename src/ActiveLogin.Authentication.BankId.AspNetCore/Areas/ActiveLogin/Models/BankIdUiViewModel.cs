namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiViewModel
{
    internal BankIdUiViewModel(BankIdUiScriptConfiguration bankUiScriptConfiguration, BankIdUiScriptInitState bankIdUiScriptInitState, BankIdUiSignData? signData = null)
    {
        BankUiScriptConfiguration = bankUiScriptConfiguration;
        BankIdUiScriptInitState = bankIdUiScriptInitState;
        BankIdUiSignData = signData;
    }

    public string? LocalizedPageHeader { get; set; }
    public string? LocalizedPageTitle { get; set;  }

    public BankIdUiScriptConfiguration BankUiScriptConfiguration { get; }

    public BankIdUiScriptInitState BankIdUiScriptInitState { get; }

    public BankIdUiSignData? BankIdUiSignData { get; }
}

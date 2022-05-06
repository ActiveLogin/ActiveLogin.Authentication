using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiViewModel
{
    internal BankIdUiViewModel(
        string returnUrl,
        string cancelReturnUrl,
        string uiOptions,
        BankIdUiOptions unprotectedUiOptions,
        BankIdUiScriptOptions uiScriptOptions,
        string loginScriptOptionsJson,
        string antiXsrfRequestToken)
    {
        ReturnUrl = returnUrl;
        CancelReturnUrl = cancelReturnUrl;
        UiOptions = uiOptions;
        UnprotectedUiOptions = unprotectedUiOptions;
        UiScriptOptions = uiScriptOptions;
        LoginScriptOptionsJson = loginScriptOptionsJson;
        AntiXsrfRequestToken = antiXsrfRequestToken;
    }

    public string ReturnUrl { get; }
    public string CancelReturnUrl { get; }

    public string UiOptions { get; }
    public BankIdUiOptions UnprotectedUiOptions { get; }

    public BankIdUiScriptOptions UiScriptOptions { get; }
    public string LoginScriptOptionsJson { get; }
    public string AntiXsrfRequestToken { get; }
}

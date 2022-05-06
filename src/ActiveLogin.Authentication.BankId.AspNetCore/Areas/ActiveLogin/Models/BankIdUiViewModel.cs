using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiViewModel
{
    internal BankIdUiViewModel(
        string returnUrl,
        string cancelReturnUrl,
        string loginOptions,
        BankIdLoginOptions unprotectedLoginOptions,
        BankIdUiScriptOptions uiScriptOptions,
        string loginScriptOptionsJson,
        string antiXsrfRequestToken)
    {
        ReturnUrl = returnUrl;
        CancelReturnUrl = cancelReturnUrl;
        LoginOptions = loginOptions;
        UnprotectedLoginOptions = unprotectedLoginOptions;
        UiScriptOptions = uiScriptOptions;
        LoginScriptOptionsJson = loginScriptOptionsJson;
        AntiXsrfRequestToken = antiXsrfRequestToken;
    }

    public string ReturnUrl { get; }
    public string CancelReturnUrl { get; }

    public string LoginOptions { get; }
    public BankIdLoginOptions UnprotectedLoginOptions { get; }

    public BankIdUiScriptOptions UiScriptOptions { get; }
    public string LoginScriptOptionsJson { get; }
    public string AntiXsrfRequestToken { get; }
}

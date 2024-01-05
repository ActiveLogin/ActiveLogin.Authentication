namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLauncherCustomAppCallbackResult
{
    public BankIdLauncherCustomAppCallbackResult(string? returnUrl, BrowserReloadBehaviourOnReturnFromBankIdApp browserReloadBehaviourOnReturnFromBankIdApp = BrowserReloadBehaviourOnReturnFromBankIdApp.Default)
    {
        ReturnUrl = returnUrl;
        BrowserReloadBehaviourOnReturnFromBankIdApp = browserReloadBehaviourOnReturnFromBankIdApp;
    }

    public string? ReturnUrl { get; set; }
    public BrowserReloadBehaviourOnReturnFromBankIdApp BrowserReloadBehaviourOnReturnFromBankIdApp { get; set; }
}

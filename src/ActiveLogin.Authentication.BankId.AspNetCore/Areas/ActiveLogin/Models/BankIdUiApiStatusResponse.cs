namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiApiStatusResponse
{
    internal BankIdUiApiStatusResponse(string statusMessage, bool retryLogin)
    {
        StatusMessage = statusMessage;
        RetryLogin = retryLogin;
    }

    internal BankIdUiApiStatusResponse(bool isFinished, string? statusMessage, string? redirectUri)
    {
        IsFinished = isFinished;
        StatusMessage = statusMessage;
        RedirectUri = redirectUri;
    }

    public bool IsFinished { get; }
    public string? StatusMessage { get; }
    public string? RedirectUri { get; }
    public bool RetryLogin { get; }


    public static BankIdUiApiStatusResponse Finished(string redirectUri)
    {
        return new BankIdUiApiStatusResponse(true, null, redirectUri);
    }

    public static BankIdUiApiStatusResponse Pending(string statusMessage)
    {
        return new BankIdUiApiStatusResponse(false, statusMessage, null);
    }

    public static BankIdUiApiStatusResponse Retry(string statusMessage)
    {
        return new BankIdUiApiStatusResponse(statusMessage, true);
    }
}

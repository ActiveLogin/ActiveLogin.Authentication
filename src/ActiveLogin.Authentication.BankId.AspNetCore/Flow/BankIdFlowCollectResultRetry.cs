namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

public class BankIdFlowCollectResultRetry : BankIdFlowCollectResult
{
    public BankIdFlowCollectResultRetry(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    public string StatusMessage { get; init; }
}

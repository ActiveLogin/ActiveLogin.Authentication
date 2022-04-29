namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow;

public class BankIdFlowCollectResultPending : BankIdFlowCollectResult
{
    public BankIdFlowCollectResultPending(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    public string StatusMessage { get; init; }
}

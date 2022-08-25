namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowCollectResultPending : BankIdFlowCollectResult
{
    public BankIdFlowCollectResultPending(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    public string StatusMessage { get; init; }
}

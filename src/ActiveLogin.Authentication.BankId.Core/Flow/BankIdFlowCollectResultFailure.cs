namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowCollectResultFailure : BankIdFlowCollectResult
{
    public BankIdFlowCollectResultFailure(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    public string StatusMessage { get; init; }
}

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public abstract class BankIdFlowCollectResult
    { }

    public class BankIdFlowCollectResultComplete : BankIdFlowCollectResult
    {
        public BankIdFlowCollectResultComplete(CompletionData completionData)
        {
            CompletionData = completionData;
        }

        public CompletionData CompletionData { get; set; }
    }

    public class BankIdFlowCollectResultPending : BankIdFlowCollectResult
    {
        public BankIdFlowCollectResultPending(string statusMessage)
        {
            StatusMessage = statusMessage;
        }

        public string StatusMessage { get; init; }
    }

    public class BankIdFlowCollectResultRetry : BankIdFlowCollectResult
    {
        public BankIdFlowCollectResultRetry(string statusMessage)
        {
            StatusMessage = statusMessage;
        }

        public string StatusMessage { get; init; }
    }

    public class BankIdFlowCollectResultFailure : BankIdFlowCollectResult
    {
        public BankIdFlowCollectResultFailure(string statusMessage)
        {
            StatusMessage = statusMessage;
        }

        public string StatusMessage { get; init; }
    }
}

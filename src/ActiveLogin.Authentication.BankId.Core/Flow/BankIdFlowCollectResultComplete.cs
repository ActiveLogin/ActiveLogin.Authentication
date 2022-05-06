using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowCollectResultComplete : BankIdFlowCollectResult
{
    public BankIdFlowCollectResultComplete(CompletionData completionData)
    {
        CompletionData = completionData;
    }

    public CompletionData CompletionData { get; set; }
}

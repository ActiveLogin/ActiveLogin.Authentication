namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public class BankIdFlowCollectResultFailure : BankIdFlowCollectResult
    {
        public BankIdFlowCollectResultFailure(string statusMessage)
        {
            StatusMessage = statusMessage;
        }

        public string StatusMessage { get; init; }
    }
}

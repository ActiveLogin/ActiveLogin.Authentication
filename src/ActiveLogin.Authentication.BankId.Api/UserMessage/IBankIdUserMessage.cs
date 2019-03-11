using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api.UserMessage
{
    public interface IBankIdUserMessage
    {
        MessageShortName GetMessageShortNameForCollectResponse(CollectStatus collectStatus, CollectHintCode hintCode,
            bool authPersonalIdentityNumberProvided, bool accessedFromMobileDevice);

        MessageShortName GetMessageShortNameForErrorResponse(ErrorCode errorCode);
    }
}

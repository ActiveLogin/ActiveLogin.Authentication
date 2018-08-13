using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api.UserMessage
{
    public interface IGrandIdUserMessage
    {
        MessageShortName GetMessageShortNameForCollectResponse(CollectStatus collectStatus, CollectHintCode hintCode, bool authPersonalIdentityNumberProvided, bool accessedFromMobileDevice);
        MessageShortName GetMessageShortNameForErrorResponse(ErrorCode errorCode);
    }
}
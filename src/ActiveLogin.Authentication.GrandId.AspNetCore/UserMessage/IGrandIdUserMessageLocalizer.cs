using ActiveLogin.Authentication.GrandId.Api.UserMessage;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Resources
{
    public interface IGrandIdUserMessageLocalizer
    {
        string GetLocalizedString(MessageShortName messageShortName);
    }
}

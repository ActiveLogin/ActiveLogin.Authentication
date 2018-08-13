using ActiveLogin.Authentication.GrandId.Api.UserMessage;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Resources
{
    public class GrandIdUserMessageStringLocalizer : IGrandIdUserMessageLocalizer
    {
        private readonly IStringLocalizer<AspNetCore.GrandIdAuthenticationHandler> _localizer;

        public GrandIdUserMessageStringLocalizer(IStringLocalizer<AspNetCore.GrandIdAuthenticationHandler> localizer)
        {
            _localizer = localizer;
        }

        public string GetLocalizedString(MessageShortName messageShortName)
        {
            return _localizer[$"GrandIdUserMessage_ShortName_{messageShortName}"];
        }
    }
}
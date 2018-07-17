using ActiveLogin.Authentication.BankId.Api.UserMessage;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Resources
{
    public class BankIdUserMessageStringLocalizer : IBankIdUserMessageLocalizer
    {
        private readonly IStringLocalizer<AspNetCore.BankIdAuthenticationHandler> _localizer;

        public BankIdUserMessageStringLocalizer(IStringLocalizer<AspNetCore.BankIdAuthenticationHandler> localizer)
        {
            _localizer = localizer;
        }

        public string GetLocalizedString(MessageShortName messageShortName)
        {
            return _localizer[$"BankIdUserMessage_ShortName_{messageShortName}"];
        }
    }
}
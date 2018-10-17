using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserMessage
{
    public interface IBankIdUserMessageLocalizer
    {
        string GetLocalizedString(MessageShortName messageShortName);
    }
}

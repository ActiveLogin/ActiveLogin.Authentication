using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Resources
{
    public interface IBankIdUserMessageLocalizer
    {
        string GetLocalizedString(MessageShortName messageShortName);
    }
}

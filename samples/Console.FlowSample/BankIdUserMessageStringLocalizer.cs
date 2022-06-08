using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

public class BankIdUserMessageStringLocalizer : IBankIdUserMessageLocalizer
{
    public string GetLocalizedString(MessageShortName messageShortName)
    {
        return messageShortName.ToString();
    }
}

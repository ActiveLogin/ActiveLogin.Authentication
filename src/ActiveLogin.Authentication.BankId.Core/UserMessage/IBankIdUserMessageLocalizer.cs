using ActiveLogin.Authentication.BankId.Api.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.UserMessage;

/// <summary>
/// Resolves localized texts for the different BankID messages.
/// </summary>
public interface IBankIdUserMessageLocalizer
{
    /// <summary>
    /// Returns a localized string.
    /// </summary>
    /// <param name="messageShortName">The message identifier.</param>
    /// <returns></returns>
    string GetLocalizedString(MessageShortName messageShortName);
}

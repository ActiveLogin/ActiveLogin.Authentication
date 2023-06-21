namespace ActiveLogin.Authentication.BankId.Api.Models;

public static class VerifyAuthenticationExtensions
{
    /// <summary>
    /// Timestamp indicating date and time in UTC when digital ID cardholder was identified using BankID.
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    public static DateTime GetIdentifiedAtDateTime(this VerifyAuthentication authentication)
    {
        return BankIdApiConverters.ParseIso8601DateTime(authentication.IdentifiedAt);
    }
}

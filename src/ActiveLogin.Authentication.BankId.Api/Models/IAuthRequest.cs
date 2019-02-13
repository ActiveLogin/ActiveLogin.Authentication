namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public interface IAuthRequest
    {
        string EndUserIp { get; }
        string PersonalIdentityNumber { get; }
        Requirement Requirement { get; }
    }
}
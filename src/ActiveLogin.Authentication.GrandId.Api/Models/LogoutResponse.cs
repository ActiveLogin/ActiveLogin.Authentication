namespace ActiveLogin.Authentication.GrandId.Api.Models;

public class LogoutResponse
{
    internal LogoutResponse(LogoutFullResponse fullResponse)
    {
        SessionDeleted = fullResponse.SessionDeleted?.Equals("1") ?? false;
    }

    public LogoutResponse(bool sessionDeleted)
    {
        SessionDeleted = sessionDeleted;
    }

    public bool SessionDeleted { get; }
}
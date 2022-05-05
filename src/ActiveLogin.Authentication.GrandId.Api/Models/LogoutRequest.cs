namespace ActiveLogin.Authentication.GrandId.Api.Models;

public class LogoutRequest
{
    public LogoutRequest(string sessionId)
    {
        SessionId = sessionId;
    }

    public string SessionId { get; }
}
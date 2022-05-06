using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models;

public class GrandIdState
{
    public GrandIdState(AuthenticationProperties authenticationProperties)
    {
        AuthenticationProperties = authenticationProperties;
    }

    public AuthenticationProperties AuthenticationProperties { get; }
}
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public class BankIdUiAuthState
{
    public BankIdUiAuthState(AuthenticationProperties authenticationProperties)
    {
        AuthenticationProperties = authenticationProperties;
    }

    public AuthenticationProperties AuthenticationProperties { get; }
}

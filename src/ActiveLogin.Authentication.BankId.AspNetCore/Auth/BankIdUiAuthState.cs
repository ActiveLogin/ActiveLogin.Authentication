using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public class BankIdUiAuthState : BankIdUiState
{
    public override BankIdStateType Type => BankIdStateType.Auth;

    public BankIdUiAuthState(AuthenticationProperties authenticationProperties)
    {
        AuthenticationProperties = authenticationProperties;
    }

    public AuthenticationProperties AuthenticationProperties { get; }
}

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiState
{
    public BankIdUiState(AuthenticationProperties authenticationProperties)
    {
        AuthenticationProperties = authenticationProperties;
    }

    public AuthenticationProperties AuthenticationProperties { get; }
}

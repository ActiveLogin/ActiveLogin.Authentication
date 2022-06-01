namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public abstract class BankIdUiState
{
    public abstract BankIdStateType Type { get; }
}

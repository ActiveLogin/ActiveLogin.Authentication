namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public enum BankIdStateType
{
    Auth = 0,
    Sign = 1,
}

public abstract class BankIdUiState
{
    public abstract BankIdStateType Type { get; }
}

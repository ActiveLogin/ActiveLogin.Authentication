using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiStateProtector
{
    string Protect(BankIdUiState state);
    BankIdUiState Unprotect(string protectedState);
}

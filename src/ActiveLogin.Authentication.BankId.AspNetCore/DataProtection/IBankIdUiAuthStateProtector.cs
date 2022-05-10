using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiAuthStateProtector
{
    string Protect(BankIdUiAuthState state);
    BankIdUiAuthState Unprotect(string protectedState);
}

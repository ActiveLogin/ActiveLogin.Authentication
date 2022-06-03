using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiOptionsProtector
{
    string Protect(BankIdUiOptions uiOptions);
    BankIdUiOptions Unprotect(string protectedUiOptions);
}

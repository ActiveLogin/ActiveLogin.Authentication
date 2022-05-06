using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdLoginOptionsProtector
{
    string Protect(BankIdLoginOptions loginOptions);
    BankIdLoginOptions Unprotect(string protectedLoginOptions);
}

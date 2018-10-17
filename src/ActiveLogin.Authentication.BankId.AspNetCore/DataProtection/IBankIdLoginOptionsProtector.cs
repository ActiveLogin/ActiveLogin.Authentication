using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection
{
    public interface IBankIdLoginOptionsProtector
    {
        string Protect(BankIdLoginOptions loginOptions);
        BankIdLoginOptions Unprotect(string protectedLoginOptions);
    }
}
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdLoginResultProtector
{
    string Protect(BankIdLoginResult loginResult);
    BankIdLoginResult Unprotect(string protectedLoginResult);
}

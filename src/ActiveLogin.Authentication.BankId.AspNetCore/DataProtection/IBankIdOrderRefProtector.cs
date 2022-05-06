using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdOrderRefProtector
{
    string Protect(BankIdOrderRef orderRef);
    BankIdOrderRef Unprotect(string protectedOrderRef);
}

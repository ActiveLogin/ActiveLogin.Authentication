using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiOrderRefProtector
{
    string Protect(BankIdUiOrderRef orderRef);
    BankIdUiOrderRef Unprotect(string protectedOrderRef);
}

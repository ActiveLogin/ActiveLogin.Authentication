namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiOptionsProtector
{
    string Protect(BankIdUiOptions uiOptions);
    BankIdUiOptions Unprotect(string protectedUiOptions);
}

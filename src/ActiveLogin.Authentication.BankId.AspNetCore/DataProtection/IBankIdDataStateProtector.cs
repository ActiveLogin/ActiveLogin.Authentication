namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdDataStateProtector<TModel>
{
    string Protect(TModel model);
    TModel Unprotect(string protectedModel);
}

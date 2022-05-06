namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiAuthResult : BankIdUiResult
{
    public BankIdUiAuthResult(bool isSuccessful, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
        : base(isSuccessful, bankIdOrderRef, personalIdentityNumber, name, givenName, surname)
    {
    }

    public static BankIdUiAuthResult Success(string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
    {
        return new BankIdUiAuthResult(true, bankIdOrderRef, personalIdentityNumber, name, givenName, surname);
    }
}

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdLoginResult
{
    internal BankIdLoginResult(bool isSuccessful, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
    {
        IsSuccessful = isSuccessful;

        BankIdOrderRef = bankIdOrderRef;

        PersonalIdentityNumber = personalIdentityNumber;

        Name = name;
        GivenName = givenName;
        Surname = surname;
    }

    public static BankIdLoginResult Success(string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
    {
        return new BankIdLoginResult(true, bankIdOrderRef, personalIdentityNumber, name, givenName, surname);
    }

    public bool IsSuccessful { get; }

    public string BankIdOrderRef { get; }

    public string PersonalIdentityNumber { get; }

    public string Name { get; }
    public string GivenName { get; }
    public string Surname { get; }
}

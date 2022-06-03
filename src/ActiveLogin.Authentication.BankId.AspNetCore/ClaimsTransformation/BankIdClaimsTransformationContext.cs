using System.Security.Claims;

using ActiveLogin.Authentication.BankId.AspNetCore.Auth;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

public class BankIdClaimsTransformationContext
{
    internal BankIdClaimsTransformationContext(BankIdAuthOptions bankIdAuthOptions, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
    {
        BankIdAuthOptions = bankIdAuthOptions;

        BankIdOrderRef = bankIdOrderRef;

        PersonalIdentityNumber = personalIdentityNumber;

        Name = name;
        GivenName = givenName;
        Surname = surname;
    }

    public List<Claim> Claims { get; set; } = new();

    public BankIdAuthOptions BankIdAuthOptions { get; }

    public string BankIdOrderRef { get; }

    public string PersonalIdentityNumber { get; }

    public string Name { get; }
    public string GivenName { get; }
    public string Surname { get; }
}

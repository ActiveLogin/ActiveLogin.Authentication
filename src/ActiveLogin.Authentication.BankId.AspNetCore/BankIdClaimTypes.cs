namespace ActiveLogin.Authentication.BankId.AspNetCore;

/// <summary>
/// Claims issued by ActiveLogin.BankId.
/// </summary>
public static class BankIdClaimTypes
{
    public const string Role = "role";

    public const string Subject = "sub";
    public const string AuthenticationMethod = "amr";
    public const string IdentityProvider = "idp";
    public const string Expires = "exp";

    public const string Name = "name";
    public const string GivenName = "given_name";
    public const string FamilyName = "family_name";
        
    public const string SwedishPersonalIdentityNumber = "swedish_personal_identity_number";
}

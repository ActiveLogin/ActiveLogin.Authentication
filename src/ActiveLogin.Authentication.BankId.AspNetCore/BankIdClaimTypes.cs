namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdClaimTypes
    {
        public const string Role = "role";

        public const string Subject = "sub";
        public const string AuthenticationMethod = "amr";
        public const string IdentityProvider = "idp";

        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";

        public const string Gender = "gender";
        public const string BirthDate = "birthdate";

        public static string SwedishPersonalIdentityNumber = "swedish_personal_identity_number";
    }
}
namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginResult
    {
        public BankIdLoginResult(bool isSuccessful, string personalIdentityNumber, string name, string givenName, string surname)
        {
            IsSuccessful = isSuccessful;
            PersonalIdentityNumber = personalIdentityNumber;
            Name = name;
            GivenName = givenName;
            Surname = surname;
        }

        public static BankIdLoginResult Success(string personalIdentityNumber, string name, string givenName, string surname)
        {
            return new BankIdLoginResult(true, personalIdentityNumber, name, givenName, surname);
        }

        public bool IsSuccessful { get; }

        public string PersonalIdentityNumber { get; }

        public string Name { get; }
        public string GivenName { get; }
        public string Surname { get; }
    }
}
namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginResult
    {
        public bool IsSuccessful { get; set; }

        public string PersonalIdentityNumber { get; set; }

        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }

        public static BankIdLoginResult Success(string personalIdentityNumber, string name, string givenName, string surname)
        {
            return new BankIdLoginResult
            {
                IsSuccessful = true,

                PersonalIdentityNumber = personalIdentityNumber,

                Name = name,
                GivenName = givenName,
                Surname = surname
            };
        }

        public static BankIdLoginResult Fail()
        {
            return new BankIdLoginResult
            {
                IsSuccessful = false
            };
        }
    }
}
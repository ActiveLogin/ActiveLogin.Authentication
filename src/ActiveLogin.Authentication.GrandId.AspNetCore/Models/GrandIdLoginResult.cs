namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginResult
    {
        public bool IsSuccessful { get; set; }

        public string PersonalIdentityNumber { get; set; }

        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }

        public static GrandIdLoginResult Success(string personalIdentityNumber, string name, string givenName, string surname)
        {
            return new GrandIdLoginResult
            {
                IsSuccessful = true,

                PersonalIdentityNumber = personalIdentityNumber,

                Name = name,
                GivenName = givenName,
                Surname = surname
            };
        }

        public static GrandIdLoginResult Fail()
        {
            return new GrandIdLoginResult
            {
                IsSuccessful = false
            };
        }
    }
}
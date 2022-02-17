using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthRequestContext
    {
        public BankIdAuthRequestContext(string endUserIp, string? personalIdentityNumber, Requirement requirement)
        {
            EndUserIp = endUserIp;
            PersonalIdentityNumber = personalIdentityNumber;
            Requirement = requirement;
        }

        public string EndUserIp { get; private set; }
       
        public string? PersonalIdentityNumber { get; private set; }

        public Requirement Requirement { get; private set; }
    }
}

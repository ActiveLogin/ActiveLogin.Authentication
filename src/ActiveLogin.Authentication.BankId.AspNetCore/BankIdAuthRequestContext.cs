using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthRequestContext
    {
        public BankIdAuthRequestContext(string endUserIp, Requirement requirement)
        {
            EndUserIp = endUserIp;
            Requirement = requirement;
        }

        public string EndUserIp { get; private set; }
       
        public Requirement Requirement { get; private set; }
    }
}

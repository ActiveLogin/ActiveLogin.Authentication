using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Core;

public class BankIdAuthRequestContext
{
    public BankIdAuthRequestContext(string endUserIp, Requirement requirement)
    {
        EndUserIp = endUserIp;
        Requirement = requirement;
    }

    public string EndUserIp { get; }
       
    public Requirement Requirement { get; }
}

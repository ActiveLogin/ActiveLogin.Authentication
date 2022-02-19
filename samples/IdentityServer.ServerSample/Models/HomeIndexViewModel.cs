using System.Security.Claims;

namespace IdentityServer.ServerSample.Models;

public class HomeIndexViewModel
{
    public HomeIndexViewModel(string givenName, string familyName, string name, string personalIdentityNumber, string birthdate, string gender, IEnumerable<Claim> claims)
    {
        GivenName = givenName;
        FamilyName = familyName;
        Name = name;
        PersonalIdentityNumber = personalIdentityNumber;
        Birthdate = birthdate;
        Gender = gender;
        Claims = claims;
    }

    public string GivenName { get; }
    public string FamilyName { get; }
    public string Name { get; }
    public string PersonalIdentityNumber { get; }
    public string Birthdate { get; }
    public string Gender { get; }

    public IEnumerable<Claim> Claims { get; }
}

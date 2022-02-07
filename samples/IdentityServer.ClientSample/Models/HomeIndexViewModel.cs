using System.Security.Claims;

namespace IdentityServer.ClientSample.Models;

public class HomeIndexViewModel
{
    public HomeIndexViewModel(string givenName, string familyName, string name, string swedishPersonalIdentityNumber, string birthdate, string gender, IEnumerable<Claim> claims)
    {
        GivenName = givenName;
        FamilyName = familyName;
        Name = name;
        SwedishPersonalIdentityNumber = swedishPersonalIdentityNumber;
        Birthdate = birthdate;
        Gender = gender;
        Claims = claims;
    }

    public string GivenName { get; }
    public string FamilyName { get; }
    public string Name { get; }
    public string SwedishPersonalIdentityNumber { get; }
    public string Birthdate { get; }
    public string Gender { get; }

    public IEnumerable<Claim> Claims { get; }
}

namespace Standalone.MvcSample.Models;

public class VerifyResultModel
{
    public VerifyResultModel(string personalIdentityNumber, string givenName, string surname, string name, int age, DateTime identifiedAt, DateTime verifiedAt)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        GivenName = givenName;
        Surname = surname;
        Name = name;
        Age = age;

        IdentifiedAt = identifiedAt;
        VerifiedAt = verifiedAt;
    }

    public string PersonalIdentityNumber { get; }
    public string GivenName { get; }
    public string Surname { get; }
    public string Name { get; }
    public int Age { get; }

    public DateTime IdentifiedAt { get; }
    public DateTime VerifiedAt { get; }
}

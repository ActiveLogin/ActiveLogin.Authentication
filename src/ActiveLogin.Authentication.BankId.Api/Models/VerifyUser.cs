using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information related to the authenticated ID cardholder.
/// </summary>
public class VerifyUser
{
    public VerifyUser(string personalIdentityNumber, string name, string givenName, string surname, int age)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        Name = name;
        GivenName = givenName;
        Surname = surname;
        Age = age;
    }

    /// <summary>
    /// The ID number of the digital ID cardholder. The ID number is a Swedish personal identity number (12 digits).
    /// </summary>
    [JsonPropertyName("personalNumber")]
    public string PersonalIdentityNumber { get; }

    /// <summary>
    /// The digital ID cardholder's given name and surname.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// The digital ID cardholder's given name.
    /// </summary>
    [JsonPropertyName("givenName")]
    public string GivenName { get; }

    /// <summary>
    /// The digital ID cardholder's surname.
    /// </summary>
    [JsonPropertyName("surname")]
    public string Surname { get; }

    /// <summary>
    /// The digital ID cardholder's age.
    /// </summary>
    [JsonPropertyName("age")]
    public int Age { get; }
}
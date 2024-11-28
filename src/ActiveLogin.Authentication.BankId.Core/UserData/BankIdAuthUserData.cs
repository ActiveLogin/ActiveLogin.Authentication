using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.UserData;

public class BankIdAuthUserData
{
    public string? UserVisibleData { get; set; }
    public byte[]? UserNonVisibleData { get; set; }
    public string? UserVisibleDataFormat { get; set; }

    /// <summary>
    /// The personal identity number allowed to confirm the identification. If a BankID with another personal identity number
    /// attempts to confirm the identification, it will fail. If left empty any personal identity number will be allowed.
    /// </summary>
    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; set; }
}

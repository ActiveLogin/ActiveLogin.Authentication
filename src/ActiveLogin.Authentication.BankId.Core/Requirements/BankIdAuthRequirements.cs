using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Requirements;

public class BankIdAuthRequirements
{
    /// <summary>
    /// The personal identity number allowed to confirm the identification.
    /// If a BankID with another personal identity number attempts to confirm the identification, it will fail.
    /// If left empty any personal identity number will be allowed.
    /// </summary>
    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; set; }

    /// <summary>
    /// Whether the user needs to confirm their identity with a valid Swedish passport or national ID card to complete the order.
    /// No identity confirmation is required by default.
    /// </summary>
    public bool? RequireMrtd { get; set; }

    /// <summary>
    /// Users are required to confirm the order with their security code even if they have biometrics activated.
    /// </summary>
    public bool? RequirePinCode { get; set; }
}

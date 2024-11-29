using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdSignData
{
    public BankIdSignData(string userVisibleData)
    {
        UserVisibleData = userVisibleData;
    }

    public string UserVisibleData { get; set; }
    public string? UserVisibleDataFormat { get; set; }

    public byte[]? UserNonVisibleData { get; set; }

    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; set; }
    public bool? RequireMrtd { get; set; }
    public bool? RequirePinCode { get; set; }

    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}

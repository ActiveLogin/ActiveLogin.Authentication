namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignProperties
{
    public BankIdSignProperties(string userVisibleData)
    {
        UserVisibleData = userVisibleData;
    }

    public string UserVisibleData { get; set; }
    public string? UserVisibleDataFormat { get; set; }

    public byte[]? UserNonVisibleData { get; set; }

    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}

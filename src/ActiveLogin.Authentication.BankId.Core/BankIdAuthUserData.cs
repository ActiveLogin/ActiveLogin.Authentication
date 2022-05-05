namespace ActiveLogin.Authentication.BankId.Core;

public class BankIdAuthUserData
{
    public string? UserVisibleData { get; set; }
    public byte[]? UserNonVisibleData { get; set; }
    public string? UserVisibleDataFormat { get; set; }
}
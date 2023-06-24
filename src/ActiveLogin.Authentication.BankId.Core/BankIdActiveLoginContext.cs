namespace ActiveLogin.Authentication.BankId.Core;

public class BankIdActiveLoginContext
{
    public string ActiveLoginProductName { get; set; } = string.Empty;
    public string ActiveLoginProductVersion { get; set; } = string.Empty;

    public string BankIdApiEnvironment { get; set; } = string.Empty;
    public string BankIdAppApiVersion { get; set; } = string.Empty;
    public string BankIdVerifyApiVersion { get; set; } = string.Empty;
}

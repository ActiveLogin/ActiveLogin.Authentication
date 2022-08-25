namespace ActiveLogin.Authentication.BankId.AzureKeyVault;

public class ClientCertificateFromAzureKeyVaultOptions
{
    // KeyVault reference
    public string? AzureKeyVaultUri { get; set; }
    public string? AzureKeyVaultSecretName { get; set; }

    // When using specific managed identity client id
    public string? AzureManagedIdentityClientId { get; set; }

    // When using client secret
    public string? AzureAdTenantId { get; set; }
    public string? AzureAdClientId { get; set; }
    public string? AzureAdClientSecret { get; set; }
}

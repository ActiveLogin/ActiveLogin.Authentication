namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public class ClientCertificateFromAzureKeyVaultOptions
    {
        public UseManagedIdentity UseManagedIdentity { get; set; }

        public string? AzureAdTenantId { get; set; }
        public string? AzureAdClientId { get; set; }
        public string? AzureAdClientSecret { get; set; }

        public string? AzureKeyVaultUri { get; set; }
        public string? AzureKeyVaultSecretName { get; set; }
        public string? AZURE_CLIENT_ID { get; set; }
    }
    public enum UseManagedIdentity
    {
        None,
        SystemAssigned,
        UserAssigned
    }
}

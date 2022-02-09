namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public class ClientCertificateFromAzureKeyVaultOptions
    {
        public bool UseManagedIdentity { get; set; }
        public ManagedIdentityType ManagedIdentityType { get; set; } = ManagedIdentityType.SystemAssigned;
        public string? ManagedIdentityUserAssignedClientId { get; set; }

        public string? AzureAdTenantId { get; set; }
        public string? AzureAdClientId { get; set; }
        public string? AzureAdClientSecret { get; set; }

        public string? AzureKeyVaultUri { get; set; }
        public string? AzureKeyVaultSecretName { get; set; }
    }
}

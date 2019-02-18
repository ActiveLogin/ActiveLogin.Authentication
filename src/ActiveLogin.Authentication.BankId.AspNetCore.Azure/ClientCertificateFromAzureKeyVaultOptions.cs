namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public class ClientCertificateFromAzureKeyVaultOptions
    {
        public string AzureAdClientId { get; set; }
        public string AzureAdClientSecret { get; set; }
        public string AzureKeyVaultSecretIdentifier { get; set; }
    }

    public class ClientCertificateFromManagedIdentityAzureKeyVaultOptions
    {
        public string AzureKeyVaultUri { get; set; }
        public string CertificateIdentifier { get; set; }
    }
}
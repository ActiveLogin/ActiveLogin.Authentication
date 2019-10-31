namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public class ClientCertificateFromAzureKeyVaultOptions
    {
        public string AzureAdClientId { get; set; }
        public string AzureAdClientSecret { get; set; }
        public string AzureKeyVaultIdentifier { get; set; }
        public bool UseManagedIdentity { get; set; }
    }
}

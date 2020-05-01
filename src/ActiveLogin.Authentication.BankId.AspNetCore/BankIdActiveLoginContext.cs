namespace Microsoft.Extensions.DependencyInjection
{
    internal class BankIdActiveLoginContext
    {
        public string ActiveLoginProductName { get; set; } = string.Empty;
        public string ActiveLoginProductVersion { get; set; } = string.Empty;

        public string BankIdApiEnvironment { get; set; } = string.Empty;
        public string BankIdApiVersion { get; set; } = string.Empty;
    }
}

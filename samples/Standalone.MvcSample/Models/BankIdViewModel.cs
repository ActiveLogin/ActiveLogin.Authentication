namespace Standalone.MvcSample.Models;

public class BankIdViewModel
{
    public BankIdViewModel(IEnumerable<ExternalProvider> externalProviders, string returnUrl)
    {
        ExternalProviders = externalProviders;
        ReturnUrl = returnUrl;
    }

    public IEnumerable<ExternalProvider> ExternalProviders { get; }
    public string ReturnUrl { get; }
}

namespace Standalone.MvcSample.Models;

public class ExternalProvider
{
    public ExternalProvider(string displayName, string bankIdScheme)
    {
        DisplayName = displayName;
        BankIdScheme = bankIdScheme;
    }

    public string DisplayName { get; }
    public string BankIdScheme { get; }
}

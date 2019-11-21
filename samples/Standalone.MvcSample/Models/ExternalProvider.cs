namespace Standalone.MvcSample.Models
{
    public class ExternalProvider
    {
        public ExternalProvider(string displayName, string authenticationScheme)
        {
            DisplayName = displayName;
            AuthenticationScheme = authenticationScheme;
        }

        public string DisplayName { get; }
        public string AuthenticationScheme { get; }
    }
}
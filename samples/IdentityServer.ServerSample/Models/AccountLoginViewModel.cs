using System.Collections.Generic;

namespace IdentityServer.ServerSample.Models
{
    public class AccountLoginViewModel
    {
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public string ReturnUrl { get; set; }
    }
}

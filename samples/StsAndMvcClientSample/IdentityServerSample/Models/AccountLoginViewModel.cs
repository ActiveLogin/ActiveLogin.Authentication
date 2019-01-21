using System.Collections.Generic;

namespace IdentityServerSample.Models
{
    public class AccountLoginViewModel
    {
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public string ReturnUrl { get; set; }
    }
}
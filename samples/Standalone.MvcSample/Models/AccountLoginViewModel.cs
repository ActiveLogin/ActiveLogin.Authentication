using System.Collections.Generic;

namespace Standalone.MvcSample.Models
{
    public class AccountLoginViewModel
    {
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public string ReturnUrl { get; set; }
    }
}
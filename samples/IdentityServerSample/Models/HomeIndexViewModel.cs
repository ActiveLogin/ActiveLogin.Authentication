using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServerSample.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}
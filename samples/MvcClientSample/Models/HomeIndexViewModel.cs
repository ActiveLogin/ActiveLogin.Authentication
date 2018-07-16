using System.Collections.Generic;
using System.Security.Claims;

namespace MvcClientSample.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}
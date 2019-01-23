using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer.ClientSample.Models
{
    public class HomeIndexViewModel
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Name { get; set; }
        public string SwedishPersonalIdentityNumber { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}
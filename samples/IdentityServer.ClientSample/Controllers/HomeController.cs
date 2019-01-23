using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer.ClientSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ClientSample.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();
            return View(new HomeIndexViewModel()
            {
                Name = GetClaimValue(claims, "name"),
                GivenName = GetClaimValue(claims, "given_name"),
                FamilyName = GetClaimValue(claims, "family_name"),
                SwedishPersonalIdentityNumber = GetClaimValue(claims, "swedish_personal_identity_number"),
                Birthdate = GetClaimValue(claims, "birthdate"),
                Gender = GetClaimValue(claims, "gender"),
                Claims = claims
            });
        }

        private string GetClaimValue(IEnumerable<Claim> claims, string type, string fallback = "-")
        {
            return claims.FirstOrDefault(x => x.Type == type)?.Value ?? fallback;
        }
    }
}

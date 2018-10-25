using Microsoft.AspNetCore.Mvc;

namespace MvcClientSample.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Logout()
        {
            return new SignOutResult(new[] { "Cookies", "oidc" });
        }
    }
}

using IdentityServerSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerSample.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View(new HomeIndexViewModel()
            {
                Claims = User.Claims
            });
        }
    }
}
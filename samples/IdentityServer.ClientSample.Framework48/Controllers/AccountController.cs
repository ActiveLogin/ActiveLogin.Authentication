using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentityServer.ClientSample.Framework48.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public void Login(string returnUrl = "/")
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge();
                return;
            }
            Response.Redirect("/");
        }
        public void LogOff()
        {
            if (Request.IsAuthenticated)
            {
                var authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
            }
            Response.Redirect("/");
        }
        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}

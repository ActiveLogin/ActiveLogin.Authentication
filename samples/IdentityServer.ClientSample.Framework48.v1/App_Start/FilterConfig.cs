using System.Web;
using System.Web.Mvc;

namespace IdentityServer.ClientSample.Framework48.v1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

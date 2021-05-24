using System.Configuration;

using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

using Microsoft.Owin.Security.OpenIdConnect;  // OpenAthens.Owin.Security.OpenIdConnect;

using Owin;

namespace IdentityServer.ClientSample.Framework48.v1
{
    public partial class Startup
    {
        public static string OidcAuthority = ConfigurationManager.AppSettings["oidc:Authority"];
        public static string OidcRedirectUrl = ConfigurationManager.AppSettings["oidc:RedirectUrl"];
        public static string OidcClientId = ConfigurationManager.AppSettings["oidc:ClientId"];
        public static string OidcClientSecret = ConfigurationManager.AppSettings["oidc:ClientSecret"];
        public void ConfigureAuthentication(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = OidcAuthority,
                ClientId = OidcClientId,
                ClientSecret = OidcClientSecret,
                /*GetClaimsFromUserInfoEndpoint = true,*/  // ?? This was from an example at docs.openathens.net, I opted for the ms version instead so...
                PostLogoutRedirectUri = OidcRedirectUrl,
                RedirectUri = OidcRedirectUrl,
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = OpenIdConnectScope.OpenId
            };
            app.UseOpenIdConnectAuthentication(oidcOptions);
        }
    }
}

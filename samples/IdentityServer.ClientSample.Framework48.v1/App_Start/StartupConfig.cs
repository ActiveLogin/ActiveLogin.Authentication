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
            
            var cookieOptions = new CookieAuthenticationOptions();
            cookieOptions.AuthenticationType = CookieAuthenticationDefaults.AuthenticationType;
            cookieOptions.CookieName = "aspnetmvcclient";

            app.UseCookieAuthentication(cookieOptions);

            //app.UseCookieAuthentication(new CookieAuthenticationOptions());

            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                Authority = OidcAuthority,
                ClientId = OidcClientId,
                ClientSecret = OidcClientSecret,

                ResponseType = "code id_token", // OpenIdConnectResponseType.Code

                PostLogoutRedirectUri = OidcRedirectUrl,
                RedirectUri = OidcRedirectUrl,
                
                Scope = $"{OpenIdConnectScope.OpenIdProfile} personalidentitynumber",   // "openid profile personalidentitynumber"

                UseTokenLifetime = true
            };

            app.UseOpenIdConnectAuthentication(oidcOptions);

            /* 
             
             This is the corresponding code from the ASP.NET Core version of the initialization
            ====================================================================================
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => { options.Cookie.Name = "aspnetmvcclient"; })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = Configuration["ActiveLogin:IdentityProvider:Authority"];

                    options.ClientId = Configuration["ActiveLogin:MvcClient:ClientId"];
                    options.ClientSecret = Configuration["ActiveLogin:MvcClient:ClientSecret"];

                    options.ResponseType = "code id_token";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("personalidentitynumber");

                    options.UseTokenLifetime = true;
                });
              
             */
        }
    }
}

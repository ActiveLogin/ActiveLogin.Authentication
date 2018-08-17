using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api.UserMessage;
using ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Resources;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IGrandIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IJsonSerializer _jsonSerializer;

        public GrandIdController(IAntiforgery antiforgery, IGrandIdUserMessageLocalizer grandIdUserMessageLocalizer, IJsonSerializer jsonSerializer)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = grandIdUserMessageLocalizer;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<ActionResult> Login(string returnUrl)
        {
            //if (!Url.IsLocalUrl(returnUrl))
            //{
            //    throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            //}
            var absoluteUri = string.Concat(
                        Request.Scheme,
                        "://",
                        Request.Host.ToUriComponent(),
                        Request.PathBase.ToUriComponent());
            var cookies = Request.Cookies;
            //var routeUrl = Url.RouteUrl("GrandId/InitializeAsync", new { *Route parameters here * });
            var actionUrl = Url.Action("Initialize", "Api");
            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var redirectUrl = "loginpage.html";
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(absoluteUri),
                };
                client.DefaultRequestHeaders.Add("RequestVerificationToken", antiforgeryTokens.RequestToken);
                //returnUrl = absoluteUri + Url.Action("Verify", "Api");
                var request = new GrandIdLoginApiInitializeRequest() { ReturnUrl = returnUrl, DeviceOption = Api.Models.DeviceOption.ChooseDevice };
                var requestJson = _jsonSerializer.Serialize(request);
                var requestContent = GetJsonStringContent(requestJson);
                var response = await client.PostAsync(actionUrl, requestContent);
                response.EnsureSuccessStatusCode();
                var data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var loginResponse = _jsonSerializer.Deserialize<GrandIdLoginApiInitializeResponse>(data);
                redirectUrl = loginResponse.RedirectUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Redirect(redirectUrl);
            //var props = new AuthenticationProperties
            //{
            //    RedirectUri = redirectUrl,
            //    Items =
            //    {
            //        { "returnUrl",  Url.Action(nameof(Verify)) }
            //    }
            //};
            //return Challenge(props, "grandid");
            //return View(new GrandIdLoginViewModel
            //{
            //    ReturnUrl = returnUrl,
            //    AntiXsrfRequestToken = antiforgeryTokens.RequestToken

            //});
        }

        public async Task<ActionResult> Verify(string grandidsession)
        {
            var absoluteUri = string.Concat(
                        Request.Scheme,
                        "://",
                        Request.Host.ToUriComponent(),
                        Request.PathBase.ToUriComponent());

            //var routeUrl = Url.RouteUrl("GrandId/InitializeAsync", new { *Route parameters here * });
            var actionUrl = Url.Action("Session", "Api");
            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            GrandIdLoginApiSessionResponse stateData;
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(absoluteUri)
                };
                var request = new GrandIdLoginApiSessionRequest() { SessionId = grandidsession, DeviceOption = Api.Models.DeviceOption.ChooseDevice };
                var requestJson = _jsonSerializer.Serialize(request);
                var requestContent = GetJsonStringContent(requestJson);
                var response = await client.PostAsync(actionUrl, requestContent);
                response.EnsureSuccessStatusCode();
                var data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                stateData = _jsonSerializer.Deserialize<GrandIdLoginApiSessionResponse>(data);

                //var extUser = result.Principal;
                //var extUserId = extUser.FindFirst(ClaimTypes.NameIdentifier);
                //var issuer = extUserId.Issuer;

                // provisioning logic happens here...

                var claims = new List<Claim>
            {
                new Claim("name", stateData.UserAttributes.Name),
                new Claim("role", "Geek")
            };

                var ci = new ClaimsIdentity(claims, "password", "name", "role");
                var p = new ClaimsPrincipal(ci);

                await HttpContext.SignInAsync(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            return Redirect(redirectUrl);

            //return View(new GrandIdLoginViewModel
            //{
            //    ReturnUrl = returnUrl,
            //    AntiXsrfRequestToken = antiforgeryTokens.RequestToken

            //});
        }
        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }
    }
}
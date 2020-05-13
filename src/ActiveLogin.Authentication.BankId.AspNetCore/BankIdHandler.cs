using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdHandler : RemoteAuthenticationHandler<BankIdOptions>
    {
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;
        private readonly IBankIdEventTrigger _bankIdEventTrigger;

        public BankIdHandler(
            IOptionsMonitor<BankIdOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IBankIdLoginResultProtector loginResultProtector,
            IBankIdEventTrigger bankIdEventTrigger)
            : base(options, loggerFactory, encoder, clock)
        {
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdEventTrigger = bankIdEventTrigger;
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var state = GetStateFromCookie();
            if (state == null)
            {
                return HandleRemoteAuthenticateFail("Invalid state cookie");
            }

            DeleteStateCookie();

            var loginResultProtected = Request.Query["loginResult"];
            if (string.IsNullOrEmpty(loginResultProtected))
            {
                return HandleRemoteAuthenticateFail("Missing login result");
            }

            var loginResult = _loginResultProtector.Unprotect(loginResultProtected);
            if (loginResult == null || !loginResult.IsSuccessful)
            {
                return HandleRemoteAuthenticateFail("Invalid login result");
            }

            var properties = state.AuthenticationProperties;
            var ticket = GetAuthenticationTicket(loginResult, properties);

            _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateSuccessEvent(
                ticket,
                SwedishPersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber)
            ));

            return Task.FromResult(HandleRequestResult.Success(ticket));
        }

        private async Task<HandleRequestResult> HandleRemoteAuthenticateFail(string reason)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateFailureEvent(reason));

            return HandleRequestResult.Fail(reason);
        }

        private AuthenticationTicket GetAuthenticationTicket(BankIdLoginResult loginResult, AuthenticationProperties properties)
        {
            DateTimeOffset? expiresUtc = null;
            if (Options.TokenExpiresIn.HasValue)
            {
                expiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
                properties.ExpiresUtc = expiresUtc;
            }

            var claims = GetClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, BankIdClaimTypes.Name, BankIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetClaims(BankIdLoginResult loginResult, DateTimeOffset? expiresUtc)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber);
            var claims = new List<Claim>
            {
                new Claim(BankIdClaimTypes.Subject, personalIdentityNumber.To12DigitString()),

                new Claim(BankIdClaimTypes.Name, loginResult.Name),
                new Claim(BankIdClaimTypes.FamilyName, loginResult.Surname),
                new Claim(BankIdClaimTypes.GivenName, loginResult.GivenName),

                new Claim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.To10DigitString())
            };

            AddOptionalClaims(claims, personalIdentityNumber, expiresUtc);

            return claims;
        }

        private void AddOptionalClaims(List<Claim> claims, SwedishPersonalIdentityNumber personalIdentityNumber, DateTimeOffset? expiresUtc)
        {
            if (expiresUtc.HasValue)
            {
                claims.Add(new Claim(BankIdClaimTypes.Expires, JwtSerializer.GetExpires(expiresUtc.Value)));
            }

            if (Options.IssueAuthenticationMethodClaim)
            {
                claims.Add(new Claim(BankIdClaimTypes.AuthenticationMethod, Options.AuthenticationMethodName));
            }

            if (Options.IssueIdentityProviderClaim)
            {
                claims.Add(new Claim(BankIdClaimTypes.IdentityProvider, Options.IdentityProviderName));
            }

            if (Options.IssueGenderClaim)
            {
                var jwtGender = JwtSerializer.GetGender(personalIdentityNumber.GetGenderHint());
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(BankIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                var jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.GetDateOfBirthHint());
                claims.Add(new Claim(BankIdClaimTypes.Birthdate, jwtBirthdate));
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var loginOptions = new BankIdLoginOptions(
                Options.BankIdCertificatePolicies,
                GetSwedishPersonalIdentityNumber(properties),
                Options.BankIdAllowChangingPersonalIdentityNumber,
                Options.BankIdAutoLaunch,
                Options.BankIdAllowBiometric,
                Options.BankIdUseQrCode,
                GetCancelReturnUrl(properties)
            );
            var loginUrl = GetLoginUrl(loginOptions);
            Response.Redirect(loginUrl);

            await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetChallangeSuccessEvent(loginOptions));
        }

        private static SwedishPersonalIdentityNumber? GetSwedishPersonalIdentityNumber(AuthenticationProperties properties)
        {
            bool TryGetPinString(out string? s)
            {
                return properties.Items.TryGetValue(BankIdConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber, out s);
            }

            if (TryGetPinString(out var swedishPersonalIdentityNumber) && !string.IsNullOrWhiteSpace(swedishPersonalIdentityNumber))
            {
                return SwedishPersonalIdentityNumber.Parse(swedishPersonalIdentityNumber);
            }

            return null;
        }

        private string GetCancelReturnUrl(AuthenticationProperties properties)
        {
            // Default to root if no return url is set
            var cancelReturnUrl = properties.Items.ContainsKey("returnUrl") ? properties.Items["returnUrl"] : "/";


            // If cancel url is set, it overrides the regular return url
            if (properties.Items.TryGetValue("cancelReturnUrl", out var cancelUrl))
            {
                cancelReturnUrl = cancelUrl;
            }

            // If we are using other device authentication and manual PIN entry we do not redirect back to
            // returnUrl. Instead we let the GUI decide what to display. Preferably the PIN entry form.
            if (Scheme.Name.Equals(BankIdDefaults.OtherDeviceAuthenticationScheme) && !Options.BankIdUseQrCode)
            {
                cancelReturnUrl = string.Empty;
            }

            return cancelReturnUrl;
        }

        private string GetLoginUrl(BankIdLoginOptions loginOptions)
        {
            var pathBase = Context.Request.PathBase;
            var loginUrl = pathBase.Add(Options.LoginPath);
            var returnUrl = UrlEncoder.Encode(pathBase.Add(Options.CallbackPath));
            var protectedOptions = UrlEncoder.Encode(_loginOptionsProtector.Protect(loginOptions));

            return $"{loginUrl}" +
                   $"?returnUrl={returnUrl}" +
                   $"&loginOptions={protectedOptions}";
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            if (Options.StateDataFormat == null)
            {
                throw new ArgumentNullException(nameof(Options.StateDataFormat));
            }

            var state = new BankIdState(properties);
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            var cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private BankIdState? GetStateFromCookie()
        {
            if (Options.StateDataFormat == null)
            {
                throw new ArgumentNullException(nameof(Options.StateDataFormat));
            }

            var protectedState = Request.Cookies[Options.StateCookie.Name];
            if (string.IsNullOrEmpty(protectedState))
            {
                return null;
            }

            var state = Options.StateDataFormat.Unprotect(protectedState);
            return state;
        }

        private void DeleteStateCookie()
        {
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
        }
    }
}

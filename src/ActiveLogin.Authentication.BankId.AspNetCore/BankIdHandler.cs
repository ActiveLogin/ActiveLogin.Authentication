using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdHandler : RemoteAuthenticationHandler<BankIdOptions>
    {
        private const string DefaultCancelUrl = "/";

        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;
        private readonly IBankIdEventTrigger _bankIdEventTrigger;
        private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;

        public BankIdHandler(
            IOptionsMonitor<BankIdOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IBankIdLoginResultProtector loginResultProtector,
            IBankIdEventTrigger bankIdEventTrigger,
            IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector)
            : base(options, loggerFactory, encoder, clock)
        {
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdEventTrigger = bankIdEventTrigger;
            _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var detectedDevice = GetDetectedDevice();

            var state = GetStateFromCookie();
            if (state == null)
            {
                return await HandleRemoteAuthenticateFail("Invalid state cookie", detectedDevice);
            }

            DeleteStateCookie();

            var loginResultProtected = Request.Query["loginResult"];
            if (string.IsNullOrEmpty(loginResultProtected))
            {
                return await HandleRemoteAuthenticateFail("Missing login result", detectedDevice);
            }

            var loginResult = _loginResultProtector.Unprotect(loginResultProtected);
            if (loginResult == null || !loginResult.IsSuccessful)
            {
                return await HandleRemoteAuthenticateFail("Invalid login result", detectedDevice);
            }

            var properties = state.AuthenticationProperties;
            var ticket = GetAuthenticationTicket(loginResult, properties);

            await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateSuccessEvent(
                ticket,
                PersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber),
                detectedDevice
            ));

            return HandleRequestResult.Success(ticket);
        }

        private async Task<HandleRequestResult> HandleRemoteAuthenticateFail(string reason, BankIdSupportedDevice detectedDevice)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateFailureEvent(reason, detectedDevice));

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
            var personalIdentityNumber = PersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber);
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

        private void AddOptionalClaims(List<Claim> claims, PersonalIdentityNumber personalIdentityNumber, DateTimeOffset? expiresUtc)
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
                GetPersonalIdentityNumber(properties),
                Options.BankIdAllowChangingPersonalIdentityNumber,
                Options.BankIdSameDevice,
                Options.BankIdAllowBiometric,
                Options.BankIdUseQrCode,
                GetCancelReturnUrl(properties),
                Options.StateCookie.Name ?? string.Empty
            );

            var detectedDevice = GetDetectedDevice();
            await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetChallengeSuccessEvent(detectedDevice, loginOptions));

            var loginUrl = GetLoginUrl(loginOptions);
            Response.Redirect(loginUrl);
        }

        private BankIdSupportedDevice GetDetectedDevice()
        {
            return _bankIdSupportedDeviceDetector.Detect(Request.Headers[BankIdConstants.UserAgentHttpHeaderName]);
        }

        private static PersonalIdentityNumber? GetPersonalIdentityNumber(AuthenticationProperties properties)
        {
            bool TryGetPinString(out string? s)
            {
                return properties.Items.TryGetValue(BankIdConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber, out s);
            }

            if (TryGetPinString(out var personalIdentityNumber) && !string.IsNullOrWhiteSpace(personalIdentityNumber))
            {
                return PersonalIdentityNumber.Parse(personalIdentityNumber, StrictMode.Off);
            }

            return null;
        }

        private string GetCancelReturnUrl(AuthenticationProperties properties)
        {
            // Default to root if no return url is set
            var cancelReturnUrl = properties.Items.ContainsKey("returnUrl") ? properties.Items["returnUrl"] : DefaultCancelUrl;


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

            return cancelReturnUrl ?? DefaultCancelUrl;
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
            ArgumentNullException.ThrowIfNull(Options.StateCookie.Name);
            ArgumentNullException.ThrowIfNull(Options.StateDataFormat);

            var state = new BankIdState(properties);
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            var cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private BankIdState? GetStateFromCookie()
        {
            ArgumentNullException.ThrowIfNull(Options.StateCookie.Name);
            ArgumentNullException.ThrowIfNull(Options.StateDataFormat);

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
            ArgumentNullException.ThrowIfNull(Options.StateCookie.Name);

            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
        }
    }
}

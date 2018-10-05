using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Dummy implementation that can be used for development and testing purposes.
    /// </summary>
    public class GrandIdDevelopmentApiClient : IGrandIdApiClient
    {
        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _personalIdentityNumber;

        private readonly Dictionary<string, AuthResponse> _auths = new Dictionary<string, AuthResponse>();

        public GrandIdDevelopmentApiClient() : this("GivenName", "Surname")
        {
        }

        public GrandIdDevelopmentApiClient(string givenName, string surname) : this(givenName, surname, "199908072391")
        {
        }

        public GrandIdDevelopmentApiClient(string givenName, string surname, string personalIdentityNumber)
        {
            _givenName = givenName;
            _surname = surname;
            _personalIdentityNumber = personalIdentityNumber;
        }

        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var sessionId = Guid.NewGuid().ToString();
            var response = new AuthResponse
            {
                SessionId = sessionId,
                RedirectUrl = $"{request.CallbackUrl}?grandidsession={sessionId}"
            };
            _auths.Add(sessionId, response);
            return response;
        }

        public async Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_auths.ContainsKey(request.SessionId))
            {
                throw new GrandIdApiException(ErrorCode.UNKNOWN, "SessionId not found");
            }

            var auth = _auths[request.SessionId];
            _auths.Remove(request.SessionId);
            var response = new SessionStateResponse
            {
                SessionId = auth.SessionId,
                UserAttributes = GetUserAttributes(_personalIdentityNumber)
            };

            return response;
        }

        private UserAttributes GetUserAttributes(string personalIdentityNumber)
        {
            return new UserAttributes
            {
                GivenName = _givenName,
                Surname = _surname,
                Name = $"{_givenName} {_surname}",
                PersonalIdentityNumber = personalIdentityNumber
            };
        }

        private static async Task SimulateResponseDelay()
        {
            await Task.Delay(250).ConfigureAwait(false);
        }
    }
}
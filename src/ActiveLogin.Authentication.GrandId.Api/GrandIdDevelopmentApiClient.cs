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
        private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

        private readonly Dictionary<string, ExtendedFederatedLoginResponse> _bankidFederatedLogins = new Dictionary<string, ExtendedFederatedLoginResponse>();
        private readonly Dictionary<string, FederatedDirectLoginResponse> _federatedDirectLogins = new Dictionary<string, FederatedDirectLoginResponse>();

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

        public TimeSpan Delay
        {
            get => _delay;
            set => _delay = value < TimeSpan.Zero ? TimeSpan.Zero : value;
        }

        public async Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var sessionId = Guid.NewGuid().ToString();
            var response = new BankIdFederatedLoginResponse
            {
                SessionId = sessionId,
                RedirectUrl = $"{request.CallbackUrl}?grandidsession={sessionId}"
            };
            var extendedResponse = new ExtendedFederatedLoginResponse(response, request.PersonalIdentityNumber);
            _bankidFederatedLogins.Add(sessionId, extendedResponse);
            return response;
        }

        public async Task<BankIdSessionStateResponse> BankIdGetSessionAsync(BankIdSessionStateRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_bankidFederatedLogins.ContainsKey(request.SessionId))
            {
                throw new GrandIdApiException(ErrorCode.UNKNOWN, "SessionId not found");
            }

            var auth = _bankidFederatedLogins[request.SessionId];
            _bankidFederatedLogins.Remove(request.SessionId);

            var personalIdentityNumber = !string.IsNullOrEmpty(auth.PersonalIdentityNumber) ? auth.PersonalIdentityNumber : _personalIdentityNumber;
            var response = new BankIdSessionStateResponse
            {
                SessionId = auth.BankIdFederatedLoginResponse.SessionId,
                UserAttributes = GetUserAttributes(personalIdentityNumber)
            };

            return response;
        }


        public async Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(FederatedDirectLoginRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var sessionId = Guid.NewGuid().ToString();
            var response = new FederatedDirectLoginResponse
            {
                SessionId = sessionId,
                Username = $"{_givenName.ToLower()}.{_surname.ToLower()}@example.org",
                UserAttributes = new FederatedDirectLoginUserAttributes
                {
                    GivenName = _givenName,
                    Surname = _surname,
                    MobilePhone = string.Empty,
                    SameAccountName = $"{_givenName.ToLower()}.{_surname.ToLower()}",
                    Title = "Software Developer"
                }
            };
            _federatedDirectLogins.Add(sessionId, response);
            return response;
        }


        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var sessionId = request.SessionId;

            if (_bankidFederatedLogins.ContainsKey(sessionId))
            {
                _bankidFederatedLogins.Remove(sessionId);
            }

            if (_federatedDirectLogins.ContainsKey(sessionId))
            {
                _federatedDirectLogins.Remove(sessionId);
            }

            return new LogoutResponse
            {
                SessionDeleted = true
            };
        }

        private SessionUserAttributes GetUserAttributes(string personalIdentityNumber)
        {
            return new SessionUserAttributes
            {
                GivenName = _givenName,
                Surname = _surname,
                Name = $"{_givenName} {_surname}",
                PersonalIdentityNumber = personalIdentityNumber
            };
        }

        private async Task SimulateResponseDelay()
        {
            await Task.Delay(Delay).ConfigureAwait(false);
        }

        private class ExtendedFederatedLoginResponse
        {
            public ExtendedFederatedLoginResponse(BankIdFederatedLoginResponse bankIdFederatedLoginResponse, string personalIdentityNumber)
            {
                BankIdFederatedLoginResponse = bankIdFederatedLoginResponse;
                PersonalIdentityNumber = personalIdentityNumber;
            }

            public BankIdFederatedLoginResponse BankIdFederatedLoginResponse { get; }
            public string PersonalIdentityNumber { get; }
        }
    }
}
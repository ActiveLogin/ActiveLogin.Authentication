using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Dummy implementation that simulates the GrandID API. Can be used for development and testing purposes.
    /// </summary>
    public class GrandIdSimulatedApiClient : IGrandIdApiClient
    {
        private const string DefaultGivenName = "GivenName";
        private const string DefaultSurname = "Surname";
        private const string DefaultPersonalIdentityNumber = "199908072391";

        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _personalIdentityNumber;
        private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

        private readonly Dictionary<string, ExtendedFederatedLoginResponse> _bankidFederatedLogins = new Dictionary<string, ExtendedFederatedLoginResponse>();

        public GrandIdSimulatedApiClient()
            : this(DefaultGivenName, DefaultSurname)
        {
        }

        public GrandIdSimulatedApiClient(string givenName, string surname)
            : this(givenName, surname, DefaultPersonalIdentityNumber)
        {
        }

        public GrandIdSimulatedApiClient(string givenName, string surname, string personalIdentityNumber)
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

            var personalIdentityNumber = !string.IsNullOrEmpty(request.PersonalIdentityNumber) ? request.PersonalIdentityNumber : _personalIdentityNumber;
            await EnsureNoExistingLogin(personalIdentityNumber).ConfigureAwait(false);

            var sessionId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var response = new BankIdFederatedLoginResponse(sessionId, $"{request.CallbackUrl}?grandidsession={sessionId}");
            var extendedResponse = new ExtendedFederatedLoginResponse(response, personalIdentityNumber);
            _bankidFederatedLogins.Add(sessionId, extendedResponse);

            return response;
        }

        private async Task EnsureNoExistingLogin(string personalIdentityNumber)
        {
            if (_bankidFederatedLogins.Any(x => x.Value.PersonalIdentityNumber == personalIdentityNumber))
            {
                var existingLoginSessionId = _bankidFederatedLogins.First(x => x.Value.PersonalIdentityNumber == personalIdentityNumber).Key;
                await LogoutAsync(new LogoutRequest(existingLoginSessionId)).ConfigureAwait(false);

                throw new GrandIdApiException(ErrorCode.Already_In_Progress, "A login for this user is already in progress.");
            }
        }

        public async Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_bankidFederatedLogins.ContainsKey(request.SessionId))
            {
                throw new GrandIdApiException(ErrorCode.Unknown, "SessionId not found.");
            }

            var auth = _bankidFederatedLogins[request.SessionId];
            _bankidFederatedLogins.Remove(request.SessionId);

            var personalIdentityNumber = auth.PersonalIdentityNumber;
            var userAttributes = GetUserAttributes(personalIdentityNumber);
            var response = new BankIdGetSessionResponse(auth.BankIdFederatedLoginResponse.SessionId,
                userAttributes.PersonalIdentityNumber,
                userAttributes
            );

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

            return new LogoutResponse(true);
        }

        private BankIdGetSessionUserAttributes GetUserAttributes(string personalIdentityNumber)
        {
            return new BankIdGetSessionUserAttributes(string.Empty, _givenName, _surname, $"{_givenName} {_surname}", personalIdentityNumber, string.Empty, string.Empty, string.Empty);
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
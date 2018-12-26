using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Dummy implementation that can be used for development and testing purposes.
    /// </summary>
    public class GrandIdDevelopmentApiClient : IGrandIdApiClient
    {
        private const string DefaultGivenName = "GivenName";
        private const string DefaultSurname = "Surname";
        private const string DefaultPersonalIdentityNumber = "199908072391";
        private const string DefaultMobilePhone = "0046999123456";
        private const string DefaultTitle = "Software Developer";

        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _personalIdentityNumber;
        private readonly string _mobilePhone;
        private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

        private readonly Dictionary<string, ExtendedFederatedLoginResponse> _bankidFederatedLogins = new Dictionary<string, ExtendedFederatedLoginResponse>();
        private readonly Dictionary<string, FederatedDirectLoginResponse> _federatedDirectLogins = new Dictionary<string, FederatedDirectLoginResponse>();

        public GrandIdDevelopmentApiClient()
            : this(DefaultGivenName, DefaultSurname)
        {
        }

        public GrandIdDevelopmentApiClient(string givenName, string surname)
            : this(givenName, surname, DefaultPersonalIdentityNumber)
        {
        }

        public GrandIdDevelopmentApiClient(string givenName, string surname, string personalIdentityNumber)
            : this(givenName, surname, personalIdentityNumber, DefaultMobilePhone)
        {
        }

        public GrandIdDevelopmentApiClient(string givenName, string surname, string personalIdentityNumber, string mobilePhone)
        {
            _givenName = givenName;
            _surname = surname;
            _personalIdentityNumber = personalIdentityNumber;
            _mobilePhone = mobilePhone;
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


        public async Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(FederatedDirectLoginRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var sessionId = Guid.NewGuid().ToString();
            var userAttributes = new FederatedDirectLoginUserAttributes(_mobilePhone, _givenName, _surname, request.Username, DefaultTitle);
            var response = new FederatedDirectLoginResponse(sessionId, request.Username, userAttributes);
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
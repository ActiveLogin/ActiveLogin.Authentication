using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    /// Dummy implementation that can be used for development and testing purposes.
    /// </summary>
    public class BankIdDevelopmentApiClient : IBankIdApiClient
    {
        private static readonly List<KeyValuePair<CollectStatus, CollectHintCode>> DefaultStatusesToReturn = new List<KeyValuePair<CollectStatus, CollectHintCode>>
        {
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Pending, CollectHintCode.Started),
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Pending, CollectHintCode.Started),
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Pending, CollectHintCode.UserSign),
            new KeyValuePair<CollectStatus, CollectHintCode>(CollectStatus.Complete, CollectHintCode.UserSign)
        };

        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _name;
        private readonly string _personalIdentityNumber;
        private readonly List<KeyValuePair<CollectStatus, CollectHintCode>> _statusesToReturn;

        private readonly Dictionary<string, Auth> _auths = new Dictionary<string, Auth>();
        private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

        public BankIdDevelopmentApiClient()
            : this(DefaultStatusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
            : this("GivenName", "Surname", "199908072391", statusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname)
            : this(givenName, surname, "199908072391")
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, string personalIdentityNumber)
            : this(givenName, surname, personalIdentityNumber, DefaultStatusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, string personalIdentityNumber, List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
            : this(givenName, surname, $"{givenName} {surname}", personalIdentityNumber, statusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, string name, string personalIdentityNumber, List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
        {
            _givenName = givenName;
            _surname = surname;
            _name = name;
            _personalIdentityNumber = personalIdentityNumber;
            _statusesToReturn = statusesToReturn;
        }

        public TimeSpan Delay
        {
            get => _delay;
            set => _delay = value < TimeSpan.Zero ? TimeSpan.Zero : value;
        }

        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            var personalIdentityNumber = GetPersonalIdentityNumber(request);
            await EnsureNoExistingAuth(personalIdentityNumber).ConfigureAwait(false);

            var orderRef = Guid.NewGuid().ToString();
            var auth = new Auth(orderRef, personalIdentityNumber);
            _auths.Add(orderRef, auth);

            return new AuthResponse(orderRef, Guid.NewGuid().ToString());
        }

        private string GetPersonalIdentityNumber(AuthRequest request)
        {
            if (!string.IsNullOrEmpty(request.PersonalIdentityNumber))
            {
                return request.PersonalIdentityNumber;
            }

            return _personalIdentityNumber;
        }

        private async Task EnsureNoExistingAuth(string personalIdentityNumber)
        {
            if (_auths.Any(x => x.Value.PersonalIdentityNumber == personalIdentityNumber))
            {
                var existingAuthOrderRef = _auths.First(x => x.Value.PersonalIdentityNumber == personalIdentityNumber).Key;
                await CancelAsync(new CancelRequest(existingAuthOrderRef)).ConfigureAwait(false);

                throw new BankIdApiException(new Error("AlreadyInProgress", "A login for this user is already in progress."));
            }
        }

        public async Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_auths.ContainsKey(request.OrderRef))
            {
                throw new BankIdApiException(new Error("NotFound", "OrderRef not found."));
            }

            var auth = _auths[request.OrderRef];
            var status = GetStatus(auth.CollectCalls);
            var hintCode = GetHintCode(auth.CollectCalls);
            var completionData = GetCompletionData(status, auth.PersonalIdentityNumber);

            var response = new CollectResponse(auth.OrderRef, status.ToString(), hintCode.ToString(), completionData);

            if (status == CollectStatus.Complete)
            {
                if (_auths.ContainsKey(request.OrderRef))
                {
                    _auths.Remove(request.OrderRef);
                }
            }
            else
            {
                auth.CollectCalls += 1;
            }

            return response;
        }

        private CompletionData GetCompletionData(CollectStatus status, string personalIdentityNumber)
        {
            if (status != CollectStatus.Complete)
            {
                return null;
            }

            var user = new User(personalIdentityNumber, _name, _givenName, _surname);

            return new CompletionData(user, new Device(), new Cert(), string.Empty, string.Empty);
        }

        private CollectStatus GetStatus(int collectCalls)
        {
            var index = GetStatusesToReturnIndex(collectCalls);
            return _statusesToReturn[index].Key;
        }

        private CollectHintCode GetHintCode(int collectCalls)
        {
            var index = GetStatusesToReturnIndex(collectCalls);
            return _statusesToReturn[index].Value;
        }

        private int GetStatusesToReturnIndex(int collectCalls)
        {
            return Math.Min(collectCalls, (_statusesToReturn.Count - 1));
        }

        public async Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (_auths.ContainsKey(request.OrderRef))
            {
                _auths.Remove(request.OrderRef);
            }

            return new CancelResponse();
        }

        private async Task SimulateResponseDelay()
        {
            await Task.Delay(Delay).ConfigureAwait(false);
        }

        private class Auth
        {
            public Auth(string orderRef, string personalIdentityNumber)
            {
                OrderRef = orderRef;
                PersonalIdentityNumber = personalIdentityNumber;
            }

            public string OrderRef { get; }
            public string PersonalIdentityNumber { get; }
            public int CollectCalls { get; set; }
        }
    }
}
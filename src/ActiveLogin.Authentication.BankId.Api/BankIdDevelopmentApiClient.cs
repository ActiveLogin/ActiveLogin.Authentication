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
        private readonly List<KeyValuePair<CollectStatus, CollectHintCode>> _statusesToReturn;

        private readonly Dictionary<string, Auth> _auths = new Dictionary<string, Auth>();

        public BankIdDevelopmentApiClient()
            : this(DefaultStatusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
            : this("GivenName", "Surname", "Name", statusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname)
            : this(givenName, surname, $"{givenName} {surname}")
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
            : this(givenName, surname, $"{givenName} {surname}", statusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, string name)
            : this(givenName, surname, name, DefaultStatusesToReturn)
        {
        }

        public BankIdDevelopmentApiClient(string givenName, string surname, string name, List<KeyValuePair<CollectStatus, CollectHintCode>> statusesToReturn)
        {
            _givenName = givenName;
            _surname = surname;
            _name = name;
            _statusesToReturn = statusesToReturn;
        }

        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            await EnsureNoExistingAuth(request).ConfigureAwait(false);

            var orderRef = Guid.NewGuid().ToString();
            var auth = new Auth(orderRef, request.PersonalIdentityNumber);
            _auths.Add(orderRef, auth);

            return new AuthResponse
            {
                OrderRef = orderRef
            };
        }

        private async Task EnsureNoExistingAuth(AuthRequest request)
        {
            if (_auths.Any(x => x.Value.PersonalIdentityNumber == request.PersonalIdentityNumber))
            {
                var existingAuthOrderRef = _auths.First(x => x.Value.PersonalIdentityNumber == request.PersonalIdentityNumber).Key;
                await CancelAsync(new CancelRequest(existingAuthOrderRef)).ConfigureAwait(false);
                throw new BankIdApiException(ErrorCode.AlreadyInProgress, "A login for this user is already in progress.");
            }
        }

        public async Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_auths.ContainsKey(request.OrderRef))
            {
                throw new BankIdApiException(ErrorCode.NotFound, "OrderRef not found");
            }

            var auth = _auths[request.OrderRef];
            var status = GetStatus(auth.CollectCalls);
            var hintCode = GetHintCode(auth.CollectCalls);
            var completionData = GetCompletionData(status, auth.PersonalIdentityNumber);

            var response = new CollectResponse(status, hintCode)
            {
                OrderRef = auth.OrderRef,
                CompletionData = completionData
            };

            auth.CollectCalls += 1;

            return response;
        }

        private CompletionData GetCompletionData(CollectStatus status, string personalIdentityNumber)
        {
            if (status != CollectStatus.Complete)
            {
                return null;
            }

            return new CompletionData
            {
                User = new User
                {
                    PersonalIdentityNumber = personalIdentityNumber,
                    Name = _name,
                    GivenName = _givenName,
                    Surname = _surname
                }
            };
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

        private static async Task SimulateResponseDelay()
        {
            await Task.Delay(250).ConfigureAwait(false);
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
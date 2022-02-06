using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    /// Dummy implementation that simulates the BankId API. Can be used for development and testing purposes.
    /// </summary>
    public class BankIdSimulatedApiClient : IBankIdApiClient
    {
        private const string DefaultGivenName = "GivenName";
        private const string DefaultSurname = "Surname";
        private const string DefaultPersonalIdentityNumber = "199908072391";

        private static readonly List<CollectState> DefaultCollectStates = new()
        {
            new CollectState(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
            new CollectState(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
            new CollectState(CollectStatus.Pending, CollectHintCode.Started),
            new CollectState(CollectStatus.Pending, CollectHintCode.Started),
            new CollectState(CollectStatus.Pending, CollectHintCode.UserSign),
            new CollectState(CollectStatus.Complete, CollectHintCode.UserSign)
        };

        private readonly string _givenName;
        private readonly string _surname;
        private readonly string _name;
        private readonly string _personalIdentityNumber;
        private readonly List<CollectState> _collectStates;

        private readonly Dictionary<string, Auth> _auths = new();
        private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

        public BankIdSimulatedApiClient()
            : this(DefaultCollectStates)
        {
        }

        public BankIdSimulatedApiClient(List<CollectState> collectStates)
            : this(DefaultGivenName, DefaultSurname, DefaultPersonalIdentityNumber, collectStates)
        {
        }

        public BankIdSimulatedApiClient(string givenName, string surname)
            : this(givenName, surname, DefaultPersonalIdentityNumber)
        {
        }

        public BankIdSimulatedApiClient(string givenName, string surname, string personalIdentityNumber)
            : this(givenName, surname, personalIdentityNumber, DefaultCollectStates)
        {
        }

        public BankIdSimulatedApiClient(string givenName, string surname, string personalIdentityNumber, List<CollectState> collectStates)
            : this(givenName, surname, $"{givenName} {surname}", personalIdentityNumber, collectStates)
        {
        }

        public BankIdSimulatedApiClient(string givenName, string surname, string name, string personalIdentityNumber)
            : this(givenName, surname, name, personalIdentityNumber, DefaultCollectStates)
        {
        }

        public BankIdSimulatedApiClient(string givenName, string surname, string name, string personalIdentityNumber, List<CollectState> collectStates)
        {
            _givenName = givenName;
            _surname = surname;
            _name = name;
            _personalIdentityNumber = personalIdentityNumber;
            _collectStates = collectStates;
        }

        public TimeSpan Delay
        {
            get => _delay;
            set => _delay = value < TimeSpan.Zero ? TimeSpan.Zero : value;
        }

        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GetOrderResponseAsync(request.PersonalIdentityNumber, request.EndUserIp).ConfigureAwait(false);
            return new AuthResponse(response.OrderRef, response.AutoStartToken, response.QrStartSecret, response.QrStartSecret);
        }

        public async Task<SignResponse> SignAsync(SignRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await GetOrderResponseAsync(request.PersonalIdentityNumber, request.EndUserIp).ConfigureAwait(false);
            return new SignResponse(response.OrderRef, response.AutoStartToken, response.QrStartToken, response.QrStartSecret);
        }

        private async Task<OrderResponse> GetOrderResponseAsync(string? personalIdentityNumber, string endUserIp)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (personalIdentityNumber == null || string.IsNullOrWhiteSpace(personalIdentityNumber))
            {
                personalIdentityNumber = _personalIdentityNumber;
            }

            await EnsureNoExistingAuth(personalIdentityNumber).ConfigureAwait(false);

            var orderRef = GetRandomToken();
            var autoStartToken = GetRandomToken();
            var qrStartToken = GetRandomToken();
            var qrStartSecret = GetRandomToken();

            var auth = new Auth(endUserIp, orderRef, personalIdentityNumber);
            _auths.Add(orderRef, auth);
            return new OrderResponse(orderRef, autoStartToken, qrStartToken, qrStartSecret);
        }

        private string GetRandomToken()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task EnsureNoExistingAuth(string personalIdentityNumber)
        {
            if (_auths.Any(x => x.Value.PersonalIdentityNumber == personalIdentityNumber))
            {
                var existingAuthOrderRef = _auths.First(x => x.Value.PersonalIdentityNumber == personalIdentityNumber).Key;
                await CancelAsync(new CancelRequest(existingAuthOrderRef)).ConfigureAwait(false);

                throw new BankIdApiException(ErrorCode.AlreadyInProgress, "A login for this user is already in progress.");
            }
        }

        public async Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            await SimulateResponseDelay().ConfigureAwait(false);

            if (!_auths.ContainsKey(request.OrderRef))
            {
                throw new BankIdApiException(ErrorCode.NotFound, "OrderRef not found.");
            }

            var auth = _auths[request.OrderRef];
            var status = GetStatus(auth.CollectCalls);
            var hintCode = GetHintCode(auth.CollectCalls);

            if (status != CollectStatus.Complete)
            {
                auth.CollectCalls += 1;
                return new CollectResponse(auth.OrderRef, status.ToString(), hintCode.ToString());
            }

            if (_auths.ContainsKey(request.OrderRef))
            {
                _auths.Remove(request.OrderRef);
            }

            var completionData = GetCompletionData(auth.EndUserIp, status, auth.PersonalIdentityNumber);

            return new CollectResponse(auth.OrderRef, status.ToString(), hintCode.ToString(), completionData);
        }

        private CompletionData? GetCompletionData(string endUserIp, CollectStatus status, string personalIdentityNumber)
        {
            if (status != CollectStatus.Complete)
            {
                return null;
            }

            var user = new User(personalIdentityNumber, _name, _givenName, _surname);
            var device = new Device(endUserIp);

            var certNow = DateTime.UtcNow;
            var certNotBefore = UnixTimestampMillisecondsFromDateTime(certNow.AddMonths(-1));
            var certNotAfter = UnixTimestampMillisecondsFromDateTime(certNow.AddMonths(1));
            var cert = new Cert(certNotBefore.ToString("D"), certNotAfter.ToString("D"));

            var signature = string.Empty; // Not implemented in the simulated client
            var ocspResponse = string.Empty; // Not implemented in the simulated client

            return new CompletionData(user, device, cert, signature, ocspResponse);
        }

        private static long UnixTimestampMillisecondsFromDateTime(DateTime dateTime)
        {
            var offset = new DateTimeOffset(dateTime);
            return offset.ToUnixTimeMilliseconds();
        }

        private CollectStatus GetStatus(int collectCalls)
        {
            var index = GetStatusesToReturnIndex(collectCalls);
            return _collectStates[index].Status;
        }

        private CollectHintCode GetHintCode(int collectCalls)
        {
            var index = GetStatusesToReturnIndex(collectCalls);
            return _collectStates[index].HintCode;
        }

        private int GetStatusesToReturnIndex(int collectCalls)
        {
            return Math.Min(collectCalls, (_collectStates.Count - 1));
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
            public Auth(string endUserIp, string orderRef, string personalIdentityNumber)
            {
                EndUserIp = endUserIp;
                OrderRef = orderRef;
                PersonalIdentityNumber = personalIdentityNumber;
            }

            public string EndUserIp { get; }

            public string OrderRef { get; }

            public string PersonalIdentityNumber { get; }

            public int CollectCalls { get; set; }
        }

        private class OrderResponse
        {
            public OrderResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret)
            {
                OrderRef = orderRef;
                AutoStartToken = autoStartToken;
                QrStartToken = qrStartToken;
                QrStartSecret = qrStartSecret;
            }

            public string OrderRef { get; }

            public string AutoStartToken { get; }

            public string QrStartToken { get; }

            public string QrStartSecret { get; }
        }

        public class CollectState
        {
            public CollectState(CollectStatus status, CollectHintCode hintCode)
            {
                Status = status;
                HintCode = hintCode;
            }

            public CollectStatus Status { get; }

            public CollectHintCode HintCode { get; }
        }
    }
}

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// Dummy implementation that simulates the BankId App API. Can be used for development and testing purposes.
/// </summary>
public class BankIdSimulatedAppApiClient : IBankIdAppApiClient
{
    public const string Version = "1.0";

    private const string DefaultGivenName = "GivenName";
    private const string DefaultSurname = "Surname";
    private const string DefaultPersonalIdentityNumber = "199908072391";
    private const string DefaultUniqueHardwareId = "OZvYM9VvyiAmG7NA5jU5zqGcVpo=";
    private const string DefaultBankIdIssueDate = "2023-01-01";

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
    private readonly string _bankIdIssueDate;
    private readonly string _uniqueHardwareId;
    private readonly List<CollectState> _collectStates;

    private readonly Dictionary<string, Session> _sessions = new();
    private TimeSpan _delay = TimeSpan.FromMilliseconds(250);

    public BankIdSimulatedAppApiClient()
        : this(DefaultCollectStates)
    {
    }

    public BankIdSimulatedAppApiClient(List<CollectState> collectStates)
        : this(DefaultGivenName, DefaultSurname, DefaultPersonalIdentityNumber, collectStates)
    {
    }

    public BankIdSimulatedAppApiClient(string givenName, string surname)
        : this(givenName, surname, DefaultPersonalIdentityNumber)
    {
    }

    public BankIdSimulatedAppApiClient(string givenName, string surname, string personalIdentityNumber)
        : this(givenName, surname, personalIdentityNumber, DefaultCollectStates)
    {
    }

    public BankIdSimulatedAppApiClient(string givenName, string surname, string personalIdentityNumber, List<CollectState> collectStates)
        : this(givenName, surname, $"{givenName} {surname}", personalIdentityNumber, DefaultBankIdIssueDate, DefaultUniqueHardwareId, collectStates)
    {
    }

    public BankIdSimulatedAppApiClient(string givenName, string surname, string name, string personalIdentityNumber)
        : this(givenName, surname, name, personalIdentityNumber, DefaultBankIdIssueDate, DefaultUniqueHardwareId, DefaultCollectStates)
    {
    }

    public BankIdSimulatedAppApiClient(string givenName, string surname, string name, string personalIdentityNumber, string bankIdIssueDate, string uniqueHardwareId, List<CollectState> collectStates)
    {
        _givenName = givenName;
        _surname = surname;
        _name = name;
        _personalIdentityNumber = personalIdentityNumber;
        _bankIdIssueDate = bankIdIssueDate;
        _uniqueHardwareId = uniqueHardwareId;
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

        var response = await GetOrderResponseAsync(request.EndUserIp, request.Requirement.Mrtd ?? false).ConfigureAwait(false);
        return new AuthResponse(response.OrderRef, response.AutoStartToken, response.QrStartToken, response.QrStartSecret);
    }

    public async Task<SignResponse> SignAsync(SignRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var response = await GetOrderResponseAsync(request.EndUserIp, request.Requirement.Mrtd ?? false).ConfigureAwait(false);
        return new SignResponse(response.OrderRef, response.AutoStartToken, response.QrStartToken, response.QrStartSecret);
    }

    private async Task<OrderResponse> GetOrderResponseAsync(string endUserIp, bool mrtd)
    {
        await SimulateResponseDelay().ConfigureAwait(false);

        await EnsureNoExistingAuth(_personalIdentityNumber).ConfigureAwait(false);

        var orderRef = GetRandomToken();
        var autoStartToken = GetRandomToken();
        var qrStartToken = GetRandomToken();
        var qrStartSecret = GetRandomToken();

        var session = new Session(endUserIp, orderRef, _personalIdentityNumber, mrtd);
        _sessions.Add(orderRef, session);
        return new OrderResponse(orderRef, autoStartToken, qrStartToken, qrStartSecret);
    }

    private string GetRandomToken()
    {
        return Guid.NewGuid().ToString();
    }

    private async Task EnsureNoExistingAuth(string personalIdentityNumber)
    {
        if (_sessions.Any(x => x.Value.PersonalIdentityNumber == personalIdentityNumber))
        {
            var existingAuthOrderRef = _sessions.First(x => x.Value.PersonalIdentityNumber == personalIdentityNumber).Key;
            await CancelAsync(new CancelRequest(existingAuthOrderRef)).ConfigureAwait(false);

            throw new BankIdApiException(ErrorCode.AlreadyInProgress, "A login for this user is already in progress.");
        }
    }

    public async Task<CollectResponse> CollectAsync(CollectRequest request)
    {
        await SimulateResponseDelay().ConfigureAwait(false);

        if (!_sessions.ContainsKey(request.OrderRef))
        {
            throw new BankIdApiException(ErrorCode.NotFound, "OrderRef not found.");
        }

        var session = _sessions[request.OrderRef];
        var status = GetStatus(session.CollectCalls);
        var hintCode = GetHintCode(session.CollectCalls);

        if (status != CollectStatus.Complete)
        {
            session.CollectCalls += 1;
            return new CollectResponse(session.OrderRef, status.ToString(), hintCode.ToString());
        }

        if (_sessions.ContainsKey(request.OrderRef))
        {
            _sessions.Remove(request.OrderRef);
        }

        var completionData = GetCompletionData(session.EndUserIp, status, session.PersonalIdentityNumber, session.Mrtd);

        return new CollectResponse(session.OrderRef, status.ToString(), hintCode.ToString(), completionData, null);
    }

    private CompletionData? GetCompletionData(string endUserIp, CollectStatus status, string personalIdentityNumber, bool mrtd)
    {
        if (status != CollectStatus.Complete)
        {
            return null;
        }

        var user = new User(personalIdentityNumber, _name, _givenName, _surname);
        var device = new Device(endUserIp, _uniqueHardwareId);
        var stepUp = mrtd ? new StepUp(true) : null;
        var signature = string.Empty; // Not implemented in the simulated client
        var ocspResponse = string.Empty; // Not implemented in the simulated client

        return new CompletionData(user, device, _bankIdIssueDate, stepUp, signature, ocspResponse);
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

        if (_sessions.ContainsKey(request.OrderRef))
        {
            _sessions.Remove(request.OrderRef);
        }

        return new CancelResponse();
    }

    private async Task SimulateResponseDelay()
    {
        await Task.Delay(Delay).ConfigureAwait(false);
    }

    private class Session
    {
        public Session(string endUserIp, string orderRef, string personalIdentityNumber, bool mrtd)
        {
            EndUserIp = endUserIp;
            OrderRef = orderRef;
            PersonalIdentityNumber = personalIdentityNumber;
            Mrtd = mrtd;
        }

        public string EndUserIp { get; }

        public string OrderRef { get; }

        public string PersonalIdentityNumber { get; }

        public bool Mrtd { get; }

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

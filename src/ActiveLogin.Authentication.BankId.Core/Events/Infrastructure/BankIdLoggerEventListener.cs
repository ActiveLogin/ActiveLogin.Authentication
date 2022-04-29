using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

/// <summary>
/// Logs event to <see cref="ILogger"/>
/// </summary>
public class BankIdLoggerEventListener : BankIdTypedEventListener
{
    private const string MissingPersonalIdentityNumber = "-";
    private readonly ILogger<BankIdLoggerEventListener> _logger;

    public BankIdLoggerEventListener(ILogger<BankIdLoggerEventListener> logger)
    {
        _logger = logger;
    }

    // ASP.NET Authentication

    public override Task HandleAspNetChallengeSuccessEvent(BankIdAspNetChallengeSuccessEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogInformation(eventId, "BankID authentication challenged");
        return Task.CompletedTask;
    }

    public override Task HandleAspNetAuthenticateSuccessEvent(BankIdAspNetAuthenticateSuccessEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogInformation(eventId, "BankID authentication ticket created");
        _logger.LogTrace(eventId, "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", e.PersonalIdentityNumber.To12DigitString() ?? MissingPersonalIdentityNumber);
        return Task.CompletedTask;
    }

    public override Task HandleAspNetAuthenticateErrorEvent(BankIdAspNetAuthenticateFailureEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogInformation(eventId, "BankID authentication had an error with reason '{ErrorReason}'", e.ErrorReason);
        return Task.CompletedTask;
    }

    // BankID API - Auth

    public override Task HandleAuthSuccessEvent(BankIdAuthSuccessEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogInformation(eventId, "BankID auth succeeded with the OrderRef '{OrderRef}'", e.OrderRef);
        _logger.LogTrace(eventId, "BankID auth succeeded for PersonalIdentityNumber '{PersonalIdentityNumber}' with the OrderRef '{OrderRef}'", e.PersonalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, e.OrderRef);
        return Task.CompletedTask;
    }

    public override Task HandleAuthFailureEvent(BankIdAuthErrorEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogError(eventId, e.BankIdApiException, "BankID auth failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
        _logger.LogTrace(eventId, "BankID auth failed for PersonalIdentityNumber '{PersonalIdentityNumber}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.PersonalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
        return Task.CompletedTask;
    }

    // BankID API - Collect

    public override Task HandleCollectPendingEvent(BankIdCollectPendingEvent e)
    {
        _logger.LogInformation(GetEventId(e), "BankID collect is pending for OrderRef '{OrderRef}' with HintCode '{CollectHintCode}'", e.OrderRef, e.HintCode);
        return Task.CompletedTask;
    }

    public override Task HandleCollectFailureEvent(BankIdCollectFailureEvent e)
    {
        _logger.LogInformation(GetEventId(e), "BankID collect failed for OrderRef '{OrderRef}' with the HintCode '{CollectHintCode}'", e.OrderRef, e.HintCode);
        return Task.CompletedTask;
    }

    public override Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e)
    {
        var eventId = GetEventId(e);
        _logger.LogInformation(eventId, "BankID collect is completed for OrderRef '{OrderRef}'", e.OrderRef);
        _logger.LogTrace(eventId, "BankID collect is completed for OrderRef '{OrderRef}' with User (PersonalIdentityNumber: '{UserPersonalIdentityNumber}'; GivenName: '{UserGivenName}'; Surname: '{UserSurname}'; Name: '{UserName}'), Signature '{Signature}' and OcspResponse '{OcspResponse}'", e.OrderRef, e.CompletionData.User.PersonalIdentityNumber, e.CompletionData.User.GivenName, e.CompletionData.User.Surname, e.CompletionData.User.Name, e.CompletionData.Signature, e.CompletionData.OcspResponse);
        return Task.CompletedTask;
    }

    public override Task HandleCollectErrorEvent(BankIdCollectErrorEvent e)
    {
        _logger.LogError(GetEventId(e), e.BankIdApiException, "BankID collect failed for OrderRef '{OrderRef}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.OrderRef, e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
        return Task.CompletedTask;
    }

    // BankID - Cancel

    public override Task HandleCancelSuccessEvent(BankIdCancelSuccessEvent e)
    {
        _logger.LogInformation(GetEventId(e), "BankID auth was cancelled with the OrderRef '{OrderRef}'", e.OrderRef);
        return Task.CompletedTask;
    }

    public override Task HandleCancelFailureEvent(BankIdCancelErrorEvent e)
    {
        _logger.LogInformation(GetEventId(e), "BankID auth cancellation for '{OrderRef}' failed with the message '{Message}'", e.OrderRef, e.BankIdApiException.Message);
        return Task.CompletedTask;
    }

    // Helpers

    private static EventId GetEventId(BankIdEvent e)
    {
        return new EventId(e.EventTypeId, e.EventTypeName);
    }
}

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

/// <summary>
/// Trigger an event.
/// </summary>
public interface IBankIdEventTrigger
{
    Task TriggerAsync(BankIdEvent bankIdEvent);
}

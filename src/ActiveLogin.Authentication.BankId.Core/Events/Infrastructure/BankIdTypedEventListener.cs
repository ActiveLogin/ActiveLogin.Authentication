namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

/// <summary>
/// Base class that allows you to override specific known events.
/// </summary>
public abstract class BankIdTypedEventListener : IBankIdEventListener
{
    public Task HandleAsync(BankIdEvent bankIdEvent) => bankIdEvent switch
    {
        BankIdAspNetChallengeSuccessEvent e => HandleAspNetChallengeSuccessEvent(e),

        BankIdAspNetAuthenticateSuccessEvent e => HandleAspNetAuthenticateSuccessEvent(e),
        BankIdAspNetAuthenticateFailureEvent e => HandleAspNetAuthenticateErrorEvent(e),

        BankIdInitializeSuccessEvent e => HandleSuccessEvent(e),
        BankIdInitializeErrorEvent e => HandleFailureEvent(e),
            
        BankIdCancelSuccessEvent e => HandleCancelSuccessEvent(e),
        BankIdCancelErrorEvent e => HandleCancelFailureEvent(e),

        BankIdCollectPendingEvent e => HandleCollectPendingEvent(e),
        BankIdCollectCompletedEvent e => HandleCollectCompletedEvent(e),
        BankIdCollectFailureEvent e => HandleCollectFailureEvent(e),
        BankIdCollectErrorEvent e => HandleCollectErrorEvent(e),

        BankIdSignFailureEvent e => HandleSignFailureEvent(e),
        BankIdSignSuccessEvent e => HandleSignSuccessEvent(e),

        _ => Task.CompletedTask
    };

    public virtual Task HandleAspNetChallengeSuccessEvent(BankIdAspNetChallengeSuccessEvent e) => Task.CompletedTask;

    public virtual Task HandleAspNetAuthenticateSuccessEvent(BankIdAspNetAuthenticateSuccessEvent e) => Task.CompletedTask;
    public virtual Task HandleAspNetAuthenticateErrorEvent(BankIdAspNetAuthenticateFailureEvent e) => Task.CompletedTask;

    public virtual Task HandleSuccessEvent(BankIdInitializeSuccessEvent e) => Task.CompletedTask;
    public virtual Task HandleFailureEvent(BankIdInitializeErrorEvent e) => Task.CompletedTask;

    public virtual Task HandleCancelSuccessEvent(BankIdCancelSuccessEvent e) => Task.CompletedTask;
    public virtual Task HandleCancelFailureEvent(BankIdCancelErrorEvent e) => Task.CompletedTask;

    public virtual Task HandleCollectPendingEvent(BankIdCollectPendingEvent e) => Task.CompletedTask;
    public virtual Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e) => Task.CompletedTask;
    public virtual Task HandleCollectFailureEvent(BankIdCollectFailureEvent e) => Task.CompletedTask;
    public virtual Task HandleCollectErrorEvent(BankIdCollectErrorEvent e) => Task.CompletedTask;

    public virtual Task HandleSignFailureEvent(BankIdSignFailureEvent e) => Task.CompletedTask;
    public virtual Task HandleSignSuccessEvent(BankIdSignSuccessEvent e) => Task.CompletedTask;

}

using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Base class that allows you to override specific known events.
    /// </summary>
    public abstract class BankIdTypedEventListener : IBankIdEventListener
    {
        public Task HandleAsync(BankIdEvent bankIdEvent) => bankIdEvent switch
        {
            BankIdAspNetAuthenticateSuccessEvent e => HandleAspNetAuthenticateSuccessEvent(e),
            BankIdAspNetAuthenticateFailureEvent e => HandleAspNetAuthenticateErrorEvent(e),

            BankIdAspNetChallangeSuccessEvent e => HandleAspNetChallengeSuccessEvent(e),

            BankIdAuthSuccessEvent e => HandleAuthSuccessEvent(e),
            BankIdAuthErrorEvent e => HandleAuthFailureEvent(e),
            
            BankIdCancelSuccessEvent e => HandleCancelSuccessEvent(e),
            BankIdCancelErrorEvent e => HandleCancelFailureEvent(e),

            BankIdCollectPendingEvent e => HandleCollectPendingEvent(e),
            BankIdCollectCompletedEvent e => HandleCollectCompletedEvent(e),
            BankIdCollectFailureEvent e => HandleCollectFailureEvent(e),
            BankIdCollectErrorEvent e => HandleCollectErrorEvent(e),

            _ => Task.CompletedTask
        };

        public virtual Task HandleAspNetAuthenticateSuccessEvent(BankIdAspNetAuthenticateSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleAspNetAuthenticateErrorEvent(BankIdAspNetAuthenticateFailureEvent e) => Task.CompletedTask;

        public virtual Task HandleAspNetChallengeSuccessEvent(BankIdAspNetChallangeSuccessEvent e) => Task.CompletedTask;

        public virtual Task HandleAuthSuccessEvent(BankIdAuthSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleAuthFailureEvent(BankIdAuthErrorEvent e) => Task.CompletedTask;

        public virtual Task HandleCancelSuccessEvent(BankIdCancelSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleCancelFailureEvent(BankIdCancelErrorEvent e) => Task.CompletedTask;

        public virtual Task HandleCollectPendingEvent(BankIdCollectPendingEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectFailureEvent(BankIdCollectFailureEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectErrorEvent(BankIdCollectErrorEvent e) => Task.CompletedTask;
    }
}

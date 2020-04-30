using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public abstract class TypedBankIdEventListener : IBankIdEventListener
    {
        public Task HandleAsync(BankIdEvent bankIdEvent) => bankIdEvent switch
        {
            BankIdAspNetAuthenticateSuccessEvent e => HandleAspNetAuthenticateSuccessEvent(e),
            BankIdAspNetAuthenticateErrorEvent e => HandleAspNetAuthenticateErrorEvent(e),

            BankIdAspNetChallangeSuccessEvent e => HandleAspNetChallengeSuccessEvent(e),

            BankIdAuthSuccessEvent e => HandleAuthSuccessEvent(e),
            BankIdAuthErrorEvent e => HandleAuthFailureEvent(e),
            
            BankIdCancelSuccessEvent e => HandleCancelSuccessEvent(e),
            BankIdCancelFailureEvent e => HandleCancelFailureEvent(e),

            BankIdCollectPendingEvent e => HandleCollectPendingEvent(e),
            BankIdCollectCompletedEvent e => HandleCollectCompletedEvent(e),
            BankIdCollectFailureEvent e => HandleCollectFailureEvent(e),
            BankIdCollectErrorEvent e => HandleCollectErrorEvent(e),

            _ => Task.CompletedTask
        };

        public virtual Task HandleAspNetAuthenticateSuccessEvent(BankIdAspNetAuthenticateSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleAspNetAuthenticateErrorEvent(BankIdAspNetAuthenticateErrorEvent e) => Task.CompletedTask;

        public virtual Task HandleAspNetChallengeSuccessEvent(BankIdAspNetChallangeSuccessEvent e) => Task.CompletedTask;

        public virtual Task HandleAuthSuccessEvent(BankIdAuthSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleAuthFailureEvent(BankIdAuthErrorEvent e) => Task.CompletedTask;

        public virtual Task HandleCancelSuccessEvent(BankIdCancelSuccessEvent e) => Task.CompletedTask;
        public virtual Task HandleCancelFailureEvent(BankIdCancelFailureEvent e) => Task.CompletedTask;

        public virtual Task HandleCollectPendingEvent(BankIdCollectPendingEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectFailureEvent(BankIdCollectFailureEvent e) => Task.CompletedTask;
        public virtual Task HandleCollectErrorEvent(BankIdCollectErrorEvent e) => Task.CompletedTask;
    }
}

using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public abstract class TypedBankIdEventListener : IBankIdEventListener
    {
        public Task HandleAsync(BankIdEvent bankIdEvent) => bankIdEvent switch
        {
            BankIdAuthenticationTicketCreatedEvent e => HandleBankIdAuthenticationTicketCreatedEvent(e),
            BankIdAuthFailureEvent e => HandleBankIdAuthFailureEvent(e),
            BankIdAuthSuccessEvent e => HandleBankIdAuthSuccessEvent(e),
            BankIdCancelFailedEvent e => HandleBankIdCancelFailedEvent(e),
            BankIdCancelSuccessEvent e => HandleBankIdCancelSuccessEvent(e),
            BankIdCollectCompletedEvent e => HandleBankIdCollectCompletedEvent(e),
            BankIdCollectErrorEvent e => HandleBankIdCollectHardFailureEvent(e),
            BankIdCollectPendingEvent e => HandleBankIdCollectPendingEvent(e),
            BankIdCollectFailureEvent e => HandleBankIdCollectSoftFailureEvent(e),
            _ => Task.CompletedTask
        };

        public virtual Task HandleBankIdAuthenticationTicketCreatedEvent(BankIdAuthenticationTicketCreatedEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdAuthFailureEvent(BankIdAuthFailureEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdAuthSuccessEvent(BankIdAuthSuccessEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCancelFailedEvent(BankIdCancelFailedEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCancelSuccessEvent(BankIdCancelSuccessEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCollectCompletedEvent(BankIdCollectCompletedEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCollectHardFailureEvent(BankIdCollectErrorEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCollectPendingEvent(BankIdCollectPendingEvent e) => Task.CompletedTask;

        public virtual Task HandleBankIdCollectSoftFailureEvent(BankIdCollectFailureEvent e) => Task.CompletedTask;
    }
}

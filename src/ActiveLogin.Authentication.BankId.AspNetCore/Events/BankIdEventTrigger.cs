using System.Collections.Generic;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public class BankIdEventTrigger : IBankIdEventTrigger
    {
        private IEnumerable<IBankIdEventListener> _listeners;

        public BankIdEventTrigger(IEnumerable<IBankIdEventListener> listeners) 
        {
            _listeners = listeners;
        }

        public void TriggerAsync(BankIdEvent bankIdEvent)
        {
            foreach (var listener in _listeners)
            {
                listener.HandleAsync(bankIdEvent);
            }
        }
    }
}

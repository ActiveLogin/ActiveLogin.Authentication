using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public class BankIdEventTrigger : IBankIdEventTrigger
    {
        private IEnumerable<IBankIdEventListener> _listeners;

        public BankIdEventTrigger(IEnumerable<IBankIdEventListener> listeners) 
        {
            _listeners = listeners;
        }

        public async Task TriggerAsync(BankIdEvent bankIdEvent)
        {
            foreach (var listener in _listeners)
            {
                await listener.HandleAsync(bankIdEvent);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public class BankIdEventTrigger : IBankIdEventTrigger
    {
        private List<IBankIdEventListener> _listeners;

        public BankIdEventTrigger(IEnumerable<IBankIdEventListener> listeners) 
        {
            _listeners = listeners.ToList();
        }

        public async Task TriggerAsync(BankIdEvent bankIdEvent)
        {
            var tasks = new List<Task>();

            foreach (var listener in _listeners)
            {
                tasks.Add(listener.HandleAsync(bankIdEvent));
            }

            await Task.WhenAll(tasks);
        }
    }
}

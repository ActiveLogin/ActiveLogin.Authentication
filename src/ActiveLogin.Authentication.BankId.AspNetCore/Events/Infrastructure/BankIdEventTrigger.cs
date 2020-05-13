using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    internal class BankIdEventTrigger : IBankIdEventTrigger
    {
        private readonly BankIdActiveLoginContext _bankIdActiveLoginContext;
        private readonly List<IBankIdEventListener> _listeners;

        public BankIdEventTrigger(IEnumerable<IBankIdEventListener> listeners, IOptions<BankIdActiveLoginContext> bankIdActiveLoginContext)
        {
            _bankIdActiveLoginContext = bankIdActiveLoginContext.Value;
            _listeners = listeners.ToList();
        }

        public async Task TriggerAsync(BankIdEvent bankIdEvent)
        {
            if (bankIdEvent == null)
            {
                throw new ArgumentNullException(nameof(bankIdEvent));
            }

            bankIdEvent.SetContext(_bankIdActiveLoginContext);

            var tasks = new List<Task>();

            foreach (var listener in _listeners)
            {
                tasks.Add(listener.HandleAsync(bankIdEvent));
            }

            await Task.WhenAll(tasks);
        }
    }
}

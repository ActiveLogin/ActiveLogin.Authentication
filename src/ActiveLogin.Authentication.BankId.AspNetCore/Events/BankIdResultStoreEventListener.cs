using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// To comply with BankID Technical requirement RFT5 you need to store the data in completion data.
    /// </summary>
    internal class BankIdResultStoreEventListener : BankIdTypedEventListener
    {
        private readonly List<IBankIdResultStore> _bankIdResultStores;

        public BankIdResultStoreEventListener(IEnumerable<IBankIdResultStore> bankIdResultStores)
        {
            _bankIdResultStores = bankIdResultStores.ToList();
        }

        public override async Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e)
        {
            var tasks = new List<Task>();

            foreach (var bankIdResultStore in _bankIdResultStores)
            {
                tasks.Add(bankIdResultStore.StoreCollectCompletedCompletionData(e.OrderRef, e.CompletionData));
            }

            await Task.WhenAll(tasks);
        }
    }
}

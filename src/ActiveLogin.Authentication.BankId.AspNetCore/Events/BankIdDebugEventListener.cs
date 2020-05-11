using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public class BankIdDebugEventListener : IBankIdEventListener
    {
        private readonly ILogger<BankIdDebugEventListener> _logger;
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public BankIdDebugEventListener(ILogger<BankIdDebugEventListener> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(BankIdEvent bankIdEvent)
        {
            var eventId = new EventId(bankIdEvent.EventTypeId, bankIdEvent.EventTypeName);
            var serializedEvent = JsonSerializer.Serialize(bankIdEvent, bankIdEvent.GetType(), SerializerOptions);

            _logger.LogDebug(eventId, serializedEvent);

            return Task.CompletedTask;
        }
    }
}

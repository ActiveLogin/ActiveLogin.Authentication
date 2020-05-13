using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    public class BankIdDebugEventListener : IBankIdEventListener
    {
        private readonly ILogger<BankIdDebugEventListener> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public BankIdDebugEventListener(ILogger<BankIdDebugEventListener> logger)
        {
            _logger = logger;

            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public Task HandleAsync(BankIdEvent bankIdEvent)
        {
            var eventId = new EventId(bankIdEvent.EventTypeId, bankIdEvent.EventTypeName);
            var serializedEvent = JsonSerializer.Serialize(bankIdEvent, bankIdEvent.GetType(), _serializerOptions);

            _logger.LogDebug(eventId, serializedEvent);

            return Task.CompletedTask;
        }
    }
}

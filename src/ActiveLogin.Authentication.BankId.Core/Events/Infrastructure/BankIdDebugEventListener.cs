using System.Text.Json;
using System.Text.Json.Serialization;

using ActiveLogin.Identity.Swedish;

using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

internal class BankIdDebugEventListener : IBankIdEventListener
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
        _serializerOptions.Converters.Add(new PersonalIdentityNumberJsonConverter());
    }

    public Task HandleAsync(BankIdEvent bankIdEvent)
    {
        var eventId = new EventId(bankIdEvent.EventTypeId, bankIdEvent.EventTypeName);
        var debugMessage = GetSerializedEventOrError(bankIdEvent);

        _logger.LogDebug(eventId, debugMessage);

        return Task.CompletedTask;
    }

    private string GetSerializedEventOrError(BankIdEvent bankIdEvent)
    {
        try
        {
            return JsonSerializer.Serialize(bankIdEvent, bankIdEvent.GetType(), _serializerOptions);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private class PersonalIdentityNumberJsonConverter : JsonConverter<PersonalIdentityNumber>
    {
        public override PersonalIdentityNumber Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            PersonalIdentityNumber.Parse(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer,
            PersonalIdentityNumber personalIdentityNumberValue,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(personalIdentityNumberValue.To12DigitString());
    }
}

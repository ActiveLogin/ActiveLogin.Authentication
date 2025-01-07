using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Identity.Swedish;

using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

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
        _serializerOptions.Converters.Add(new PersonalIdentityNumberJsonConverter());
        _serializerOptions.Converters.Add(new BankIdApiExceptionJsonConverter());
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

    // This converter is used to serialize BankIdApiException to JSON.
    // It serializes all properties of the exception, except for properties
    // of type "Type" and properties of types in the System.Reflection namespace.
    // Otherwise, this will fail when serialization and deserialization
    // with "'System.Reflection.MethodBase' instances are not supported. Path: $.TargetSite."
    private class BankIdApiExceptionJsonConverter : JsonConverter<BankIdApiException>
    {
        public override BankIdApiException Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            throw new NotImplementedException();
        public override void Write(
            Utf8JsonWriter writer,
            BankIdApiException exception,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var exceptionType = exception.GetType();
            writer.WriteString("ClassName", exceptionType.FullName);
            var properties = exceptionType.GetProperties()
                .Where(e => e.PropertyType != typeof(Type))
                .Where(e => e.PropertyType.Namespace != typeof(MemberInfo).Namespace)
                .ToList();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(exception, null);
                if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && propertyValue == null)
                {
                    continue;
                }

                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
            }
            writer.WriteEndObject();
        }
    }
}

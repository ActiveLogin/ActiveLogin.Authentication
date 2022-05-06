namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

public abstract class BankIdEvent
{
    protected BankIdEvent(int eventTypeId, string eventTypeName, BankIdEventSeverity eventSeverity)
    {
        EventTypeId = eventTypeId;
        EventTypeName = eventTypeName;
        EventSeverity = eventSeverity;
    }

    internal void SetContext(BankIdActiveLoginContext context)
    {
        ActiveLoginProductName = context.ActiveLoginProductName;
        ActiveLoginProductVersion = context.ActiveLoginProductVersion;

        BankIdApiEnvironment = context.BankIdApiEnvironment;
        BankIdApiVersion = context.BankIdApiVersion;
    }

    /// <summary>
    /// Unique id for this event type.
    /// </summary>
    public int EventTypeId { get; }

    /// <summary>
    /// Unique name for this event type.
    /// </summary>
    public string EventTypeName { get; }

    /// <summary>
    /// Event severity.
    /// </summary>
    public BankIdEventSeverity EventSeverity { get; }

    /// <summary>
    /// Product name of the product/component ActiveLogin.BankId.
    /// </summary>
    public string ActiveLoginProductName { get; private set; } = string.Empty;
    /// <summary>
    /// Product version of the product/component ActiveLogin.BankId.
    /// </summary>
    public string ActiveLoginProductVersion { get; private set; } = string.Empty;

    /// <summary>
    /// BankId environment targeted.
    /// </summary>
    public string BankIdApiEnvironment { get; private set; } = string.Empty;
    /// <summary>
    /// BankId API version targeted.
    /// </summary>
    public string BankIdApiVersion { get; private set; } = string.Empty;
}

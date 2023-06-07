namespace ActiveLogin.Authentication.BankId.AzureMonitor;

public class ApplicationInsightsBankIdEventListenerOptions
{
    /// <summary>
    /// If personal identity number (personnummer) should be logged.
    /// </summary>
    public bool LogUserPersonalIdentityNumber { get; set; } = false;

    /// <summary>
    /// If anonymized hints such as birthdate and gender should be logged from the personal identity number (personnummer).
    /// </summary>
    public bool LogUserPersonalIdentityNumberHints { get; set; } = true;

    /// <summary>
    /// If names (name, given name and surname) should be logged.
    /// </summary>
    public bool LogUserNames { get; set; } = false;

    /// <summary>
    /// If issue date of the users Bank Id should be logged.
    /// </summary>
    public bool LogUserBankIdIssueDate { get; set; } = false;

    /// <summary>
    /// If ip address of the device should be logged.
    /// </summary>
    public bool LogDeviceIpAddress { get; set; } = false;

    /// <summary>
    /// If unique hardware id of the device should be logged.
    /// </summary>
    public bool LogDeviceUniqueHardwareId { get; set; } = false;

    /// <summary>
    /// If detected user device should be logged (browser, os, os version, type).
    /// </summary>
    public bool LogUserDevice { get; set; } = true;
}

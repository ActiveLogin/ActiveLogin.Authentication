using System.Text;

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// Parsers and converters for data in the BankID API.
/// </summary>
public class BankIdApiConverters
{
    /// <summary>
    /// Parse an ISO 8601 timestamp (yyyy-MM-ddTHH:mm:ssZ) into DateTime.
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ParseIso8601DateTime(string timestamp)
    {
        return DateTime.ParseExact(timestamp, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal);
    }

    /// <summary>
    /// Parse unix timestamp from milliseconds as string.
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset ParseUnixTimestamp(string timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timestamp));
    }

    /// <summary>
    /// Parse collect status from string.
    /// </summary>
    public static CollectStatus ParseCollectStatus(string status)
    {
        return Enum.TryParse<CollectStatus>(status, true, out var parsedStatus) ? parsedStatus : CollectStatus.Unknown;
    }

    /// <summary>
    /// Parse collect hint code from string.
    /// </summary>
    public static CollectHintCode ParseCollectHintCode(string hintCode)
    {
        return Enum.TryParse<CollectHintCode>(hintCode, true, out var parsedStatus) ? parsedStatus : CollectHintCode.Unknown;
    }

    /// <summary>
    /// Parse error code from string.
    /// </summary>
    public static ErrorCode ParseErrorCode(string errorCode)
    {
        return Enum.TryParse<ErrorCode>(errorCode, true, out var parsedErrorCode) ? parsedErrorCode : ErrorCode.Unknown;
    }

    /// <summary>
    /// Parse call initiator from string.
    /// </summary>
    public static CallInitiator ParseCallInitiator(string? callInitiator)
    {
        return Enum.TryParse<CallInitiator>(callInitiator, true, out var parsedStatus) ? parsedStatus : CallInitiator.Unknown;
    }

    /// <summary>
    /// Get XML from Base64 encoded string.
    /// </summary>
    public static string GetXml(string base64EncodedXml)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedXml));
    }
}

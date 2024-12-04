using System.Text;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;
public abstract class Request
{
    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    ///
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    ///
    public Request(string endUserIp, string userVisibleData)
        : this(endUserIp, userVisibleData, null, null, null)
    {
    }

    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    ///
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    public Request(string endUserIp, string userVisibleData, byte[] userNonVisibleData)
        : this(endUserIp, userVisibleData, userNonVisibleData, null, null)
    {
    }

    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    /// 
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    public Request(string endUserIp, string userVisibleData, string userVisibleDataFormat, byte[] userNonVisibleData)
        : this(endUserIp, userVisibleData, userNonVisibleData, null, userVisibleDataFormat)
    {
    }

    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    /// 
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    public Request(string endUserIp, string userVisibleData, byte[]? userNonVisibleData, Requirement? requirement)
        : this(endUserIp, userVisibleData, userNonVisibleData, requirement, null)
    {

    }

    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    /// 
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    /// <param name="returnUrl">The URL to return to when the authentication order is completed.</param>
    /// <param name="returnRisk">If set to true, a risk indication will be included in the collect response.</param>
    /// <param name="deviceParameters">Information about the device the end user is using.</param>
    public Request(string endUserIp, string? userVisibleData, byte[]? userNonVisibleData, Requirement? requirement, string? userVisibleDataFormat, string? returnUrl = null, bool? returnRisk = null, DeviceParameters? deviceParameters = null)
    {
        if (this is SignRequest && userVisibleData == null)
        {
            throw new ArgumentNullException(nameof(userVisibleData));
        }

        EndUserIp = endUserIp ?? throw new ArgumentNullException(nameof(endUserIp));
        UserVisibleData = ToBase64EncodedString(userVisibleData);
        UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
        Requirement = requirement ?? new Requirement();
        UserVisibleDataFormat = userVisibleDataFormat;
        ReturnUrl = returnUrl;
        ReturnRisk = returnRisk;

        SetDeviceParameters(deviceParameters);

    }

    /// <summary>
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    ///
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </summary>
    [JsonPropertyName("endUserIp")]
    public string EndUserIp { get; }

    /// <summary>
    /// Requirements on how the auth or sign order must be performed.
    /// </summary>
    [JsonPropertyName("requirement"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Requirement Requirement { get; }

    /// <summary>
    /// The text can be formatted using CR, LF and CRLF for new lines.
    /// The text must be encoded as UTF-8 and then base 64 encoded.
    /// 1—1 500 characters after base 64encoding.
    ///
    /// Scenario sign: The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
    ///
    /// Scenario auth: A text that is displayed to the user during authentication with BankID, with the
    /// purpose of providing context for the authentication and to enable users to notice if
    /// there is something wrong about the identification and avoid attempted frauds.
    /// </summary>
    [JsonPropertyName("userVisibleData"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserVisibleData { get; }

    /// <summary>
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </summary>
    [JsonPropertyName("userVisibleDataFormat"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserVisibleDataFormat { get; }

    /// <summary>
    /// Data not displayed to the user.
    /// </summary>
    [JsonPropertyName("userNonVisibleData"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserNonVisibleData { get; }

    /// <summary>
    /// Orders started on the same device (started with autostart token) will call this URL when the order is completed, ignoring any return URL provided in the start URL when the BankID app was launched.
    /// </summary>
    [JsonPropertyName("returnUrl"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// If this is set to true, a risk indication will be included in the collect response when the order completes.
    /// </summary>
    [JsonPropertyName("returnRisk"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ReturnRisk { get; set; }

    [JsonPropertyName("app"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public AppDeviceParameters? AppDeviceParameters { get; set; }

    [JsonPropertyName("web"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public WebDeviceParameters? WebDeviceParameters { get; set; }

    private static string? ToBase64EncodedString(string? value)
    {
        if (value == null)
        {
            return null;
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    private static string? ToBase64EncodedString(byte[]? value)
    {
        if (value == null)
        {
            return null;
        }

        return Convert.ToBase64String(value);
    }

    /// <summary>
    /// Determines the type of device parameters and sets the correct DeviceParameters property.
    /// </summary>
    /// <param name="deviceParameters"></param>
    private void SetDeviceParameters(DeviceParameters? deviceParameters)
    {
        switch (deviceParameters)
        {
            case null:
                return;
            case AppDeviceParameters appDeviceParameters:
                AppDeviceParameters = appDeviceParameters;
                break;
            case WebDeviceParameters webDeviceParameters:
                WebDeviceParameters = webDeviceParameters;
                break;
        }
    }

}

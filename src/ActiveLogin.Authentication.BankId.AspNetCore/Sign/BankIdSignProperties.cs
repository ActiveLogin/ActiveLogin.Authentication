namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignProperties
{
    /// <summary>
    /// The properties used for signing.
    /// </summary>
    /// <param name="userVisibleData">
    /// The text can be formatted using CR, LF and CRLF for new lines.
    /// The text must be encoded as UTF-8 and then base 64 encoded.
    /// 1—1 500 characters after base 64encoding.
    ///
    /// Scenario sign: The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
    ///
    /// Scenario auth: A text that is displayed to the user during authentication with BankID, with the
    /// purpose of providing context for the authentication and to enable users to notice if
    /// there is something wrong about the identification and avoid attempted frauds.
    /// </param>
    public BankIdSignProperties(string userVisibleData)
    {
        UserVisibleData = userVisibleData;
    }

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
    public string UserVisibleData { get; set; }
    /// <summary>
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </summary>
    public string? UserVisibleDataFormat { get; set; }

    /// <summary>
    /// Data not displayed to the user.
    /// </summary>
    public byte[]? UserNonVisibleData { get; set; }

    /// <summary>
    /// A collection of items where you can store state that will be provided once the sign flow is done.
    /// </summary>
    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}

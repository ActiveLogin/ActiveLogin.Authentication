using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Whether the user needs to complete the order using a card reader for the signature.
/// </summary>
public enum CardReader {
    /// <summary>
    /// Requires confirmation with a card reader where the PIN is entered on a computer keyboard or a higher class card reader.
    /// </summary>
    class1,

    /// <summary>
    /// Requires confirmation with a card reader where the PIN is entered on the reader itself. Should be combined with certificatePolicies for a smart card to avoid undefined behavior.
    /// </summary>
    class2,
}

/// <summary>
/// Requirements on how the auth or sign order must be performed.
/// </summary>
/// <remarks>
///
/// </remarks>
/// <param name="certificatePolicies">The oid in certificate policies in the user certificate. List of String.</param>
/// <param name="pinCode">
/// Users are required to sign the transaction with their PIN
/// code, even if they have biometrics activated.
///
/// <para>If set to true, the user is required to use pin code</para>
/// <para>If set to false, the users is not required to use pin code and are allowed to use fingerprint.</para>
/// </param>
/// <param name="mrtd">
/// If present, and set to "true", the client needs to provide MRTD (Machine readable travel document)
/// information to complete the order. Only Swedish passports and national ID cards are supported.
///
/// <para>If set to true, the user is required to provide MRTD information to complete the order.</para>
/// <para>If set to false or not provided, the users is not required to provide MRTD information to complete the order.</para>
/// </param>
/// <param name="personalNumber">
/// A personal number to be used to complete the transaction. If a BankID with another personal number attempts to sign the transaction, it fails.
/// </param>
/// <param name="cardReader">
/// <para>Whether the user needs to complete the order using a card reader for the signature.</para>
/// <para>This condition should always be combined with a certificatePolicies for a smart card to avoid undefined behaviour.</para>
/// <para>No card reader is required by default.</para>
/// </param>
public class Requirement(
    List<string>? certificatePolicies = null,
    bool? pinCode = null,
    bool? mrtd = null,
    string? personalNumber = null,
    CardReader? cardReader = null
){
    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    [JsonPropertyName("certificatePolicies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? CertificatePolicies { get; } = certificatePolicies;

    /// <summary>
    /// Users are required to sign the transaction with their PIN
    /// code, even if they have biometrics activated.
    ///
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </summary>
    [JsonPropertyName("pinCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PinCode { get; } = pinCode;

    /// <summary>
    /// If present, and set to "true", the client needs to provide MRTD (Machine readable travel document)
    /// information to complete the order. Only Swedish passports and national ID cards are supported.
    ///
    /// If set to true, the user is required to provide MRTD information to complete the order.
    /// If set to false, the users is not required to provide MRTD information to complete the order.
    /// </summary>
    [JsonPropertyName("mrtd"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Mrtd { get; } = mrtd;

    /// <summary>
    /// A personal number to be used to complete the transaction. If a BankID with another personal number attempts to sign the transaction, it fails.
    /// </summary>
    [JsonPropertyName("personalNumber"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PersonalNumber { get; } = personalNumber;

    /// <summary>
    /// The card reader to use for the transaction.
    /// </summary>
    [JsonPropertyName("cardReader"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CardReader? CardReader { get; } = cardReader;
}

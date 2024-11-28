using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Requirements on how the auth or sign order must be performed.
/// </summary>
public class Requirement
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="certificatePolicies">The oid in certificate policies in the user certificate. List of String.</param>
    /// <param name="pinCode">
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </param>
    /// <param name="mrtd">
    /// If present, and set to "true", the client needs to provide MRTD (Machine readable travel document)
    /// information to complete the order. Only Swedish passports and national ID cards are supported.
    /// 
    /// If set to true, the user is required to provide MRTD information to complete the order.
    /// If set to false or not provided, the users is not required to provide MRTD information to complete the order.
    /// </param>
    /// <param name="personalNumber">
    /// A personal number to be used to complete the transaction. If a BankID with another personal number attempts to sign the transaction, it fails.
    /// </param>
    /// <param name="risk">Set the acceptable risk level for the transaction.</param>
    public Requirement(List<string>? certificatePolicies = null, string? risk = null, bool? pinCode = null, bool? mrtd = null, string? personalNumber = null)
    {
        CertificatePolicies = certificatePolicies;
        Risk = risk;
        PinCode = pinCode;
        Mrtd = mrtd;
        PersonalNumber = personalNumber;
    }

    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    [JsonPropertyName("certificatePolicies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? CertificatePolicies { get; }

    /// <summary>
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </summary>
    [JsonPropertyName("pinCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PinCode { get; }

    /// <summary>
    /// If present, and set to "true", the client needs to provide MRTD (Machine readable travel document)
    /// information to complete the order. Only Swedish passports and national ID cards are supported.
    /// 
    /// If set to true, the user is required to provide MRTD information to complete the order.
    /// If set to false, the users is not required to provide MRTD information to complete the order.
    /// </summary>
    [JsonPropertyName("mrtd"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Mrtd { get; }

    /// <summary>
    /// A personal number to be used to complete the transaction. If a BankID with another personal number attempts to sign the transaction, it fails.
    /// </summary>
    [JsonPropertyName("personalNumber"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PersonalNumber { get; set; }

    /// <summary>
    /// Set the acceptable risk level for the transaction.
    /// </summary>
    [JsonPropertyName("risk"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Risk { get; }

}

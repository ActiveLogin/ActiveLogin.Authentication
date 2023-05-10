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
    /// <param name="tokenStartRequired">
    /// If set to true, the client must have been started using the AutoStartToken.
    /// To be used if it is important that the BankID App is on the same device as the RP service.
    /// 
    /// If set to false, the client does not need to be started using the autoStartToken.
    /// </param>
    /// <param name="pinCode">
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </param>
    public Requirement(List<string>? certificatePolicies = null, bool? tokenStartRequired = null, bool? pinCode = null)
    {
        CertificatePolicies = certificatePolicies;
        TokenStartRequired = tokenStartRequired;
        PinCode = pinCode;
    }

    /// <summary>
    /// The oid in certificate policies in the user certificate. List of String.
    /// </summary>
    [JsonPropertyName("certificatePolicies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? CertificatePolicies { get; }

    /// <summary>
    /// If set to true, the client must have been started using the AutoStartToken.
    /// To be used if it is important that the BankID App is on the same device as the RP service.
    /// 
    /// If set to false, the client does not need to be started using the autoStartToken.
    /// </summary>
    [JsonPropertyName("tokenStartRequired"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? TokenStartRequired { get; }

    /// <summary>
    /// Users are required to sign the transaction with their PIN 
    /// code, even if they have biometrics activated.
    /// 
    /// If set to true, the user is required to use pin code
    /// If set to false, the users is not required to use pin code and are allowed to use fingerprint.
    /// </summary>
    [JsonPropertyName("pinCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? PinCode { get; }
}

namespace ActiveLogin.Authentication.BankId.Api.Models;

public static class CompletionDataExtensions
{
    /// <summary>
    /// The signature as XML. The content of the signature is described in BankID Signature Profile specification.
    /// </summary>
    public static string GetSignatureXml(this CompletionData completionData)
    {
        return BankIdApiConverters.GetXml(completionData.Signature);
    }
}

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiResult
{
    public BankIdUiResult(bool isSuccessful, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string signature, string ocspResponse, string detectedIpAddress)
    {
        IsSuccessful = isSuccessful;

        BankIdOrderRef = bankIdOrderRef;

        PersonalIdentityNumber = personalIdentityNumber;

        Name = name;
        GivenName = givenName;
        Surname = surname;

        Signature = signature;
        OcspResponse = ocspResponse;

        DetectedIpAddress = detectedIpAddress;
    }

    public static BankIdUiResult Success(string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string signature, string ocspResponse, string detectedIpAddress)
    {
        return new BankIdUiResult(true, bankIdOrderRef, personalIdentityNumber, name, givenName, surname, signature, ocspResponse, detectedIpAddress);
    }

    public bool IsSuccessful { get; }

    public string BankIdOrderRef { get; }

    public string PersonalIdentityNumber { get; }

    public string Name { get; }
    public string GivenName { get; }
    public string Surname { get; }

    public string Signature { get; }
    public string OcspResponse { get; }

    public string DetectedIpAddress { get; }

    internal CompletionData GetCompletionData()
    {
        return new CompletionData(
            ParseUser(this),
            ParseDevice(this),
            Signature,
            OcspResponse
        );
    }

    private static User ParseUser(BankIdUiResult uiResult) => new(uiResult.PersonalIdentityNumber, uiResult.Name, uiResult.GivenName, uiResult.Surname);
    private static Device ParseDevice(BankIdUiResult uiResult) => new(uiResult.DetectedIpAddress);
}

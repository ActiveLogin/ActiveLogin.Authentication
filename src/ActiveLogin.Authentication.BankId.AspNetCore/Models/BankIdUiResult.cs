using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiResult
{
    public BankIdUiResult(bool isSuccessful, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string bankIdIssueDate, bool mrtdVerified, string signature, string ocspResponse, string detectedIpAddress, string detectedUniqueHardwareId)
    {
        IsSuccessful = isSuccessful;

        BankIdOrderRef = bankIdOrderRef;

        PersonalIdentityNumber = personalIdentityNumber;

        Name = name;
        GivenName = givenName;
        Surname = surname;

        BankIdIssueDate = bankIdIssueDate;

        MrtdVerified = mrtdVerified;

        Signature = signature;
        OcspResponse = ocspResponse;

        DetectedIpAddress = detectedIpAddress;
        DetectedUniqueHardwareId = detectedUniqueHardwareId;
    }

    public static BankIdUiResult Success(string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string bankIdIssueDate, bool mrtdVerified, string signature, string ocspResponse, string detectedIpAddress, string detectedUniqueHardwareId)
    {
        return new BankIdUiResult(true, bankIdOrderRef, personalIdentityNumber, name, givenName, surname, bankIdIssueDate, mrtdVerified, signature, ocspResponse, detectedIpAddress, detectedUniqueHardwareId);
    }

    public bool IsSuccessful { get; }

    public string BankIdOrderRef { get; }

    public string PersonalIdentityNumber { get; }

    public string Name { get; }
    public string GivenName { get; }
    public string Surname { get; }

    public string BankIdIssueDate { get; }

    public bool MrtdVerified { get; }

    public string Signature { get; }
    public string OcspResponse { get; }

    public string DetectedIpAddress { get; }
    public string DetectedUniqueHardwareId { get; }

    internal CompletionData GetCompletionData()
    {
        return new CompletionData(
            ParseUser(this),
            ParseDevice(this),
            BankIdIssueDate,
            ParseStepUp(this),
            Signature,
            OcspResponse
        );
    }

    private static User ParseUser(BankIdUiResult uiResult) => new(uiResult.PersonalIdentityNumber, uiResult.Name, uiResult.GivenName, uiResult.Surname);
    private static Device ParseDevice(BankIdUiResult uiResult) => new(uiResult.DetectedIpAddress, uiResult.DetectedUniqueHardwareId);
    private static StepUp ParseStepUp(BankIdUiResult uiResult) => new(uiResult.MrtdVerified);
}

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiResult
{
    public BankIdUiResult(bool isSuccessful, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string signature, string ocspResponse, string certNotBefore, string certNotAfter, string detectedIpAddress)
    {
        IsSuccessful = isSuccessful;

        BankIdOrderRef = bankIdOrderRef;

        PersonalIdentityNumber = personalIdentityNumber;

        Name = name;
        GivenName = givenName;
        Surname = surname;

        Signature = signature;
        OCSPResponse = ocspResponse;

        CertNotBefore = certNotBefore;
        CertNotAfter = certNotAfter;

        DetectedIpAddress = detectedIpAddress;
    }

    public static BankIdUiResult Success(string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname, string signature, string ocspResponse, string certNotBefore, string certNotAfter, string detectedIpAddress)
    {
        return new BankIdUiResult(true, bankIdOrderRef, personalIdentityNumber, name, givenName, surname, signature, ocspResponse, certNotBefore, certNotAfter, detectedIpAddress);
    }

    public bool IsSuccessful { get; }

    public string BankIdOrderRef { get; }

    public string PersonalIdentityNumber { get; }

    public string Name { get; }
    public string GivenName { get; }
    public string Surname { get; }

    public string Signature { get; }
    public string OCSPResponse { get; }

    public string CertNotBefore { get; }
    public string CertNotAfter { get; }

    public string DetectedIpAddress { get; }
}

using System.Globalization;

using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// Dummy implementation that simulates the BankId Verify API. Can be used for development and testing purposes.
/// </summary>
public class BankIdSimulatedVerifyApiClient : IBankIdVerifyApiClient
{
    public const string Version = "1.0";

    private const string TransactionType = "ID-kort-validering";

    private const string DefaultGivenName = "GivenName";
    private const string DefaultSurname = "Surname";
    private const string DefaultPersonalIdentityNumber = "199908072391";


    private readonly string _givenName;
    private readonly string _surname;
    private readonly string _name;
    private readonly string _personalIdentityNumber;

    public BankIdSimulatedVerifyApiClient()
        : this(DefaultGivenName, DefaultSurname)
    {
    }
    
    public BankIdSimulatedVerifyApiClient(string givenName, string surname)
        : this(givenName, surname, $"{givenName} {surname}", DefaultPersonalIdentityNumber)
    {
    }

    public BankIdSimulatedVerifyApiClient(string givenName, string surname, string personalIdentityNumber)
        : this(givenName, surname, $"{givenName} {surname}", personalIdentityNumber)
    {
    }

    public BankIdSimulatedVerifyApiClient(string givenName, string surname, string name, string personalIdentityNumber)
    {
        _givenName = givenName;
        _surname = surname;
        _name = name;
        _personalIdentityNumber = personalIdentityNumber;
    }

    public Task<VerifyResponse> VerifyAsync(VerifyRequest request)
    {
        var now = DateTimeOffset.UtcNow;

        var verifyResponse = GetResponse(now);

        return Task.FromResult(verifyResponse);
    }

    private VerifyResponse GetResponse(DateTimeOffset now)
    {
        var verifyUser = GetUser(now);
        var verifyVerification = GetVerification(now);
        var verifyAuthentication = GetAuthentication(now);

        return new VerifyResponse(TransactionType, verifyUser, verifyVerification, verifyAuthentication);
    }

    private static VerifyAuthentication GetAuthentication(DateTimeOffset now)
    {
        var identifiedAt = GetIso8601Timestamp(now, TimeSpan.FromSeconds(15));
        var orderRef = GetRandomToken();
        var signature = string.Empty; // Not implemented in the simulated client
        var ocspResponse = string.Empty; // Not implemented in the simulated client

        return new VerifyAuthentication(identifiedAt, orderRef, signature, ocspResponse);
    }

    private static VerifyVerification GetVerification(DateTimeOffset now)
    {
        var verificationId = GetRandomToken();
        var verifiedAt = GetIso8601Timestamp(now, TimeSpan.FromSeconds(10));
        var signature = string.Empty; // Not implemented in the simulated client

        return new VerifyVerification(verificationId, verifiedAt, signature);
    }

    private VerifyUser GetUser(DateTimeOffset now)
    {
        var personalIdentityNumber = _personalIdentityNumber;
        var age = OptimisticGetAgeFrom12DigitSwedishIdentityNumber(now, personalIdentityNumber);

        return new VerifyUser(personalIdentityNumber, _givenName, _surname, _name, age);
    }

    private static string GetIso8601Timestamp(DateTimeOffset now, TimeSpan fromNow)
    {
        return DateTimeOffset.UtcNow.Add(fromNow).ToString("o");
    }

    private static string GetRandomToken()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Optimistic implementation that parses the age from the personal identity number.
    /// </summary>
    /// <param name="now"></param>
    /// <param name="identityNumber"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static int OptimisticGetAgeFrom12DigitSwedishIdentityNumber(DateTimeOffset now, string identityNumber)
    {
        if (identityNumber.Length != 12)
        {
            throw new ArgumentException("Invalid Swedish personal identity number. It should have 12 digits.");
        }

        var birthdateString = identityNumber.Substring(0, 8);
        if (!DateTime.TryParseExact(birthdateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthdate))
        {
            throw new ArgumentException("Invalid Swedish personal identity number. The date part could not be parsed.");
        }

        var age = now.Year - birthdate.Year;
        if (birthdate > now.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}

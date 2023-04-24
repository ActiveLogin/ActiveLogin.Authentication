namespace ActiveLogin.Authentication.BankId.Api.UserMessage;

public enum MessageShortName
{
    Unknown,

    RFA1,

    /// <summary>
    /// Special version of RFA1 we use to be explicit about scanning the QR-code.
    /// </summary>
    RFA1QR,

    RFA2,
    RFA3,
    RFA4,
    RFA5,
    RFA6,
    RFA8,
    RFA9,

    RFA13,

    RFA14A,
    RFA14B,

    RFA15A,
    RFA15B,

    RFA16,

    RFA17A,
    RFA17B,

    RFA18,
    RFA19,
    RFA20,
    RFA21,
    RFA22,
    RFA23
}

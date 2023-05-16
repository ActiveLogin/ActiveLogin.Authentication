namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Possible values of call initiator for phone authentication.
/// </summary>
public enum CollectCallInitiator
{
    /// <summary>
    /// Unknown or new call initiator.
    /// </summary>
    Unknown,

    /// <summary>
    /// User called the RP. 
    /// </summary>
    User,

    /// <summary>
    /// RP called the user.
    /// </summary>
    RP
}

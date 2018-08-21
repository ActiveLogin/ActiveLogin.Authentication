namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    /// <summary>
    /// Different options on how to initialize authentication
    /// </summary>
    public enum DeviceOption
    {
        /// <summary>
        /// Open authentication on same device
        /// </summary>
        SameDevice,
        /// <summary>
        /// Open authentication on other device
        /// </summary>
        OtherDevice,
        /// <summary>
        /// Let user choose which device to open authentication on
        /// </summary>
        ChooseDevice
    }
}

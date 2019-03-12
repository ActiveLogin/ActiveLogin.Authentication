namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdFederatedLoginRequest
    {
        public BankIdFederatedLoginRequest(string callbackUrl = null,
            bool? useChooseDevice = null,
            bool? useSameDevice = null,
            bool? askForPersonalIdentityNumber = null,
            string personalIdentityNumber = null,
            bool? requireMobileBankId = null,
            string customerUrl = null,
            bool? showGui = null,
            string signUserVisibleData = null,
            string signUserNonVisibleData = null)
        {
            CallbackUrl = callbackUrl;
            UseChooseDevice = useChooseDevice;
            UseSameDevice = useSameDevice;
            AskForPersonalIdentityNumber = askForPersonalIdentityNumber;
            PersonalIdentityNumber = personalIdentityNumber;
            RequireMobileBankId = requireMobileBankId;
            CustomerUrl = customerUrl;
            ShowGui = showGui;
            SignUserVisibleData = signUserVisibleData;
            SignUserNonVisibleData = signUserNonVisibleData;
        }

        /// <summary>
        ///     The url to return the user to.
        /// </summary>
        public string CallbackUrl { get; }

        /// <summary>
        ///     Present the user with a menu choice between "this device" and "other device" (this corresponds to setting
        ///     "UseSameDevice" to true or false).
        /// </summary>
        public bool? UseChooseDevice { get; }

        /// <summary>
        ///     Try to launch bankid automatically on the device the user is using. This can interfer with setting
        ///     "RequireMobileBankId".
        /// </summary>
        public bool? UseSameDevice { get; }

        /// <summary>
        ///     Ask the user for a personal identity number. PersonalIdentityNumber overrides this, but is required when
        ///     "UseSameDevice" is false.
        /// </summary>
        public bool? AskForPersonalIdentityNumber { get; }

        /// <summary>
        ///     The users 12 digit personal number without spaces, dashes or extra characters. Will be validated by checksum before
        ///     proceeding.
        /// </summary>
        public string PersonalIdentityNumber { get; }

        /// <summary>
        ///     If set to true, only mobile certificates will be allowed to be used (mobile apps).
        /// </summary>
        public bool? RequireMobileBankId { get; }

        /// <summary>
        ///     If wanted, this parameter can be set to a URL that will be shown as the "backwards" link on all screens.
        /// </summary>
        public string CustomerUrl { get; }

        /// <summary>
        ///     When set to false instead of a redirectUrl, returns "autoStartToken" which is used to (possibly) start BankId
        ///     yourself.
        ///     When an personal identity number is passed, no special launching except informing the user about starting bankid is
        ///     required.
        /// </summary>
        public bool? ShowGui { get; }

        /// <summary>
        ///     A string to show the user when signing.
        ///     If set signing is enabled if available instead of authentication.
        /// </summary>
        public string SignUserVisibleData { get; }

        /// <summary>
        ///     The string to actually sign in the background.
        ///     If not set, the value in userVisibleData is copied.
        /// </summary>
        public string SignUserNonVisibleData { get; }
    }
}

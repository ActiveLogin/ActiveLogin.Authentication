namespace ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor
{
    public class ApplicationInsightsBankIdEventListenerOptions
    {
        /// <summary>
        /// If personal identity number (personnummer) should be logged.
        /// </summary>
        public bool LogUserPersonalIdentityNumber { get; set; } = false;

        /// <summary>
        /// If anonymized hints such as birthdate and gender should be logged from the personal identity number (personnummer).
        /// </summary>
        public bool LogHintsFromPersonalIdentityNumber { get; set; } = false;

        /// <summary>
        /// If names (name, given name and surname) should be logged.
        /// </summary>
        public bool LogUserNames { get; set; } = false;

        /// <summary>
        /// If ip address of the device should be logged.
        /// </summary>
        public bool LogDeviceIpAddress { get; set; } = false;

        /// <summary>
        /// If cert dates (not before, not after) should be logged.
        /// </summary>
        public bool LogCertDates { get; set; } = false;
    }
}

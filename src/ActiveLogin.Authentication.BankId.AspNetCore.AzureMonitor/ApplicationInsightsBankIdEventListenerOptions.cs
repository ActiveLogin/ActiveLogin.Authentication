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
        public bool LogUserPersonalIdentityNumberHints { get; set; } = false;

        /// <summary>
        /// If names (name, given name and surname) should be logged.
        /// </summary>
        public bool LogUserNames { get; set; } = false;

        /// <summary>
        /// If ip address of the device should be logged.
        /// </summary>
        public bool LogDeviceIpAddress { get; set; } = false;

        /// <summary>
        /// If certificate dates (not before, not after) should be logged.
        /// </summary>
        public bool LogCertificateDates { get; set; } = false;

        /// <summary>
        /// If detected user device should be logged (browser, os, os version, type).
        /// </summary>
        public bool LogUserDevice { get; set; } = true;
    }
}

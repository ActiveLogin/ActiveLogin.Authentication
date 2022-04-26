using System;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginScriptOptions
    {
        internal BankIdLoginScriptOptions(string bankIdInitializeApiUrl, string bankIdStatusApiUrl, string bankIdQrCodeApiUrl, string bankIdCancelApiUrl)
        {
            BankIdInitializeApiUrl = bankIdInitializeApiUrl;
            BankIdStatusApiUrl = bankIdStatusApiUrl;
            BankIdQrCodeApiUrl = bankIdQrCodeApiUrl;
            BankIdCancelApiUrl = bankIdCancelApiUrl;
        }

        private const int MinimumStatusRefreshIntervalMs = 1000;
        private int _statusRefreshIntervalMs = BankIdDefaults.StatusRefreshIntervalMs;

        [JsonPropertyName("statusRefreshIntervalMs")]
        public int StatusRefreshIntervalMs
        {
            get => _statusRefreshIntervalMs;
            set
            {
                if (value < MinimumStatusRefreshIntervalMs)
                {
                    throw new ArgumentException("BankID does not allow collecting status more than once a second.", nameof(value));
                }

                _statusRefreshIntervalMs = value;
            }
        }

        [JsonPropertyName("qrCodeRefreshIntervalMs")]
        public int QrCodeRefreshIntervalMs { get; set; }


        [JsonPropertyName("bankIdInitializeApiUrl")]
        public string BankIdInitializeApiUrl { get; set; }

        [JsonPropertyName("bankIdStatusApiUrl")]
        public string BankIdStatusApiUrl { get; set; }

        [JsonPropertyName("bankIdQrCodeApiUrl")]
        public string BankIdQrCodeApiUrl { get; set; }

        [JsonPropertyName("bankIdCancelApiUrl")]
        public string BankIdCancelApiUrl { get; set; }


        [JsonPropertyName("initialStatusMessage")]
        public string InitialStatusMessage { get; set; } = string.Empty;

        [JsonPropertyName("unknownErrorMessage")]
        public string UnknownErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("unsupportedBrowserErrorMessage")]
        public string UnsupportedBrowserErrorMessage { get; set; } = string.Empty;
    }
}

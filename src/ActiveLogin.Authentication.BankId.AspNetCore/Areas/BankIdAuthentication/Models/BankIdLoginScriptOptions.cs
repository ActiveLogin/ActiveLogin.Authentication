using System;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginScriptOptions
    {
        internal BankIdLoginScriptOptions(string bankIdInitializeApiUrl, string bankIdStatusApiUrl, string bankIdCancelApiUrl)
        {
            BankIdInitializeApiUrl = bankIdInitializeApiUrl;
            BankIdStatusApiUrl = bankIdStatusApiUrl;
            BankIdCancelApiUrl = bankIdCancelApiUrl;
        }

        private const int MinimumRefreshIntervalMs = 1000;
        private int _refreshIntervalMs = BankIdDefaults.StatusRefreshIntervalMs;

        [JsonPropertyName("refreshIntervalMs")]
        public int RefreshIntervalMs
        {
            get => _refreshIntervalMs;
            set
            {
                if (value < MinimumRefreshIntervalMs)
                {
                    throw new ArgumentException("BankID does not allow collecting status more than once a second.", nameof(value));
                }

                _refreshIntervalMs = value;
            }
        }

        [JsonPropertyName("bankIdInitializeApiUrl")]
        public string BankIdInitializeApiUrl { get; set; }

        [JsonPropertyName("bankIdStatusApiUrl")]
        public string BankIdStatusApiUrl { get; set; }

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

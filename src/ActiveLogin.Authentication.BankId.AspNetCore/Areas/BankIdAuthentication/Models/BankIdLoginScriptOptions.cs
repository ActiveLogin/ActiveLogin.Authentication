using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    [DataContract]
    public class BankIdLoginScriptOptions
    {
        private const int MinimumRefreshIntervalMs = 1000;
        private int _refreshIntervalMs = BankIdAuthenticationDefaults.StatusRefreshIntervalMs;

        [DataMember(Name = "refreshIntervalMs")]
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

        [DataMember(Name = "bankIdInitializeApiUrl")]
        public string BankIdInitializeApiUrl { get; set; }

        [DataMember(Name = "bankIdStatusApiUrl")]
        public string BankIdStatusApiUrl { get; set; }

        [DataMember(Name = "initialStatusMessage")]
        public string InitialStatusMessage { get; set; } = string.Empty;
    }
}
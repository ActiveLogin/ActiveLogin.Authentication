using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Models
{
    [DataContract]
    public class GrandIdLoginScriptOptions
    {
        private const int MinimumRefreshIntervalMs = 1000;
        private int _refreshIntervalMs = GrandIdAuthenticationDefaults.StatusRefreshIntervalMs;

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
        public string GrandIdInitializeApiUrl { get; set; }

        [DataMember(Name = "bankIdStatusApiUrl")]
        public string GrandIdStatusApiUrl { get; set; }

        [DataMember(Name = "initialStatusMessage")]
        public string InitialStatusMessage { get; set; } = string.Empty;

        [DataMember(Name = "unknownErrorMessage")]
        public string UnknownErrorMessage { get; set; } = string.Empty;
    }
}
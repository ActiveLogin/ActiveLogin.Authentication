using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using Microsoft.ApplicationInsights;

namespace ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor
{
    public class ApplicationInsightsBankIdEventListener : TypedBankIdEventListener
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ApplicationInsightsBankIdEventListenerOptions _options;

        public ApplicationInsightsBankIdEventListener(TelemetryClient telemetryClient, ApplicationInsightsBankIdEventListenerOptions options)
        {
            _telemetryClient = telemetryClient;
            _options = options;
        }


        public override Task HandleBankIdAuthenticationTicketCreatedEvent(BankIdAuthenticationTicketCreatedEvent e)
        {
            return Track(
                e,
                personalIdentityNumber: e.PersonalIdentityNumber
            );
        }

        public override Task HandleBankIdAuthFailureEvent(BankIdAuthFailureEvent e)
        {
            return Track(
                e,
                personalIdentityNumber: e.PersonalIdentityNumber,
                exception: e.BankIdApiException
            );
        }

        public override Task HandleBankIdAuthSuccessEvent(BankIdAuthSuccessEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef }
                },
                personalIdentityNumber: e.PersonalIdentityNumber
            );
        }

        // BankID API - Collect

        public override Task HandleBankIdCollectHardFailureEvent(BankIdCollectErrorEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef }
                },
                exception: e.BankIdApiException
            );
        }

        public override Task HandleBankIdCollectSoftFailureEvent(BankIdCollectFailureEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef },
                    { "AL_CollectHintCode", e.HintCode.ToString() }
                }
            );
        }

        public override Task HandleBankIdCollectPendingEvent(BankIdCollectPendingEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef },
                    { "AL_CollectHintCode", e.HintCode.ToString() }
                }
            );
        }

        public override Task HandleBankIdCollectCompletedEvent(BankIdCollectCompletedEvent e)
        {
            var properties = new Dictionary<string, string>
            {
                { "AL_OrderRef", e.OrderRef }
            };

            if (_options.LogUserNames)
            {
                properties.Add("AL_User_Name", e.CompletionData.User.Name);
                properties.Add("AL_User_GivenName", e.CompletionData.User.GivenName);
                properties.Add("AL_User_Surname", e.CompletionData.User.Surname);
            }

            if (_options.LogDeviceIpAddress)
            {
                properties.Add("AL_Device_IpAddress", e.CompletionData.Device.IpAddress);
            }

            if (_options.LogCertDates)
            {
                properties.Add("AL_Cert_NotBefore", e.CompletionData.Cert.NotBefore);
                properties.Add("AL_Cert_NotAfter", e.CompletionData.Cert.NotAfter);
            }

            return Track(
                e,
                properties,
                personalIdentityNumber: SwedishPersonalIdentityNumber.Parse(e.CompletionData.User.PersonalIdentityNumber)
            );
        }

        // BankID - Cancel

        public override Task HandleBankIdCancelSuccessEvent(BankIdCancelSuccessEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef }
                }
            );
        }

        public override Task HandleBankIdCancelFailedEvent(BankIdCancelFailedEvent e)
        {
            return Track(
                e,
                new Dictionary<string, string>
                {
                    { "AL_OrderRef", e.OrderRef }
                },
                exception: e.Exception
            );
        }

        // Helpers

        private Task Track(BankIdEvent e, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null, SwedishPersonalIdentityNumber? personalIdentityNumber = null, Exception? exception = null)
        {
            var allProperties = properties == null ? new Dictionary<string, string>() : new Dictionary<string, string>(properties);

            allProperties.Add("AL_EventTypeName", e.EventTypeName);
            allProperties.Add("AL_EventTypeId", e.EventTypeId.ToString("D"));
            allProperties.Add("AL_EventSeverity", e.EventSeverity.ToString());

            if (personalIdentityNumber != null)
            {
                AddPersonalIdentityNumberProperties(allProperties, personalIdentityNumber);
            }

            _telemetryClient.TrackEvent(e.EventTypeName, allProperties, metrics);

            if (exception != null)
            {
                if (exception is BankIdApiException bankIdApiException)
                {
                    allProperties.Add("AL_BankIdException_ErrorCode", bankIdApiException.ErrorCode.ToString());
                    allProperties.Add("AL_BankIdException_ErrorDetails", bankIdApiException.ErrorDetails);
                }

                _telemetryClient.TrackException(exception, allProperties, metrics);
            }

            return Task.CompletedTask;
        }

        private void AddPersonalIdentityNumberProperties(Dictionary<string, string> allProperties, SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            if (_options.LogUserPersonalIdentityNumber)
            {
                allProperties.Add("AL_User_SwedishPersonalIdentityNumber", personalIdentityNumber?.To12DigitString() ?? string.Empty);
            }

            if (_options.LogHintsFromPersonalIdentityNumber)
            {
                allProperties.Add("AL_User_DateOfBirthHint", personalIdentityNumber?.GetDateOfBirthHint().ToString("yyyy-MM-dd") ?? string.Empty);
                allProperties.Add("AL_User_GenderHint", personalIdentityNumber?.GetGenderHint().ToString() ?? string.Empty);
            }
        }
    }
}

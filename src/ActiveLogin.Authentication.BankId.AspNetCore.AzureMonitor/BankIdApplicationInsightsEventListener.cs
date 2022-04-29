using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using Microsoft.ApplicationInsights;

namespace ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor;

/// <summary>
/// Listen for all events and write them to Application Insights
/// </summary>
public class BankIdApplicationInsightsEventListener : BankIdTypedEventListener
{
    private const string PropertyName_ProductName = "AL_ProductName";
    private const string PropertyName_ProductVersion = "AL_ProductVersion";
    private const string PropertyName_BankIdApiEnvironment = "AL_BankId_ApiEnvironment";
    private const string PropertyName_BankIdApiVersion = "AL_BankId_ApiVersion";

    private const string PropertyName_EventTypeName = "AL_Event_TypeName";
    private const string PropertyName_EventTypeId = "AL_Event_TypeId";
    private const string PropertyName_EventSeverity = "AL_Event_Severity";

    private const string PropertyName_ErrorReason = "AL_Error_ErrorReason";

    private const string PropertyName_LoginOptionsLaunchType = "AL_BankId_LoginOptions_LaunchType";
    private const string PropertyName_LoginOptionsUseQrCode = "AL_BankId_LoginOptions_UseQrCode";
    private const string PropertyName_BankIdErrorCode = "AL_BankId_ErrorCode";
    private const string PropertyName_BankIdErrorDetails = "AL_BankId_ErrorDetails";
    private const string PropertyName_BankIdOrderRef = "AL_BankId_OrderRef";
    private const string PropertyName_BankIdCollectHintCode = "AL_BankId_CollectHintCode";

    private const string PropertyName_BankIdUserCertNotBefore = "AL_BankId_User_CertNotBefore";
    private const string PropertyName_BankIdUserCertNotAfter = "AL_BankId_User_CertNotAfter";
    private const string PropertyName_BankIdUserDeviceIpAddress = "AL_BankId_User_DeviceIpAddress";

    private const string PropertyName_UserDeviceBrowser = "AL_User_Device_Browser";
    private const string PropertyName_UserDeviceOs = "AL_User_Device_Os";
    private const string PropertyName_UserDeviceType = "AL_User_Device_Type";
    private const string PropertyName_UserDeviceOsVersion = "AL_User_Device_OsVersion";

    private const string PropertyName_UserName = "AL_User_Name";
    private const string PropertyName_UserGivenName = "AL_User_GivenName";
    private const string PropertyName_UserSurname = "AL_User_Surname";
    private const string PropertyName_UserSwedishPersonalIdentityNumber = "AL_User_SwedishPersonalIdentityNumber";
    private const string PropertyName_UserDateOfBirthHint = "AL_User_DateOfBirthHint";
    private const string PropertyName_UserAgeHint = "AL_User_AgeHint";
    private const string PropertyName_UserGenderHint = "AL_User_GenderHint";

    private readonly TelemetryClient _telemetryClient;
    private readonly ApplicationInsightsBankIdEventListenerOptions _options;

    public BankIdApplicationInsightsEventListener(TelemetryClient telemetryClient, ApplicationInsightsBankIdEventListenerOptions options)
    {
        _telemetryClient = telemetryClient;
        _options = options;
    }

    // ASP.NET Authentication

    public override Task HandleAspNetChallengeSuccessEvent(BankIdAspNetChallengeSuccessEvent e)
    {
        return Track(
            e,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleAspNetAuthenticateSuccessEvent(BankIdAspNetAuthenticateSuccessEvent e)
    {
        return Track(
            e,
            personalIdentityNumber: e.PersonalIdentityNumber,
            detectedDevice: e.DetectedUserDevice
        );
    }

    public override Task HandleAspNetAuthenticateErrorEvent(BankIdAspNetAuthenticateFailureEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_ErrorReason, e.ErrorReason }
            },
            exception: new Exception("AspNetAuthenticateError: " + e.ErrorReason),
            detectedDevice: e.DetectedUserDevice
        );
    }

    // BankID API - Auth

    public override Task HandleAuthSuccessEvent(BankIdAuthSuccessEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef }
            },
            personalIdentityNumber: e.PersonalIdentityNumber,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleAuthFailureEvent(BankIdAuthErrorEvent e)
    {
        return Track(
            e,
            personalIdentityNumber: e.PersonalIdentityNumber,
            exception: e.BankIdApiException,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    // BankID API - Collect

    public override Task HandleCollectPendingEvent(BankIdCollectPendingEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef },
                { PropertyName_BankIdCollectHintCode, e.HintCode.ToString() }
            },
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleCollectCompletedEvent(BankIdCollectCompletedEvent e)
    {
        var properties = new Dictionary<string, string>
        {
            { PropertyName_BankIdOrderRef, e.OrderRef }
        };

        if (_options.LogUserNames)
        {
            properties.Add(PropertyName_UserName, e.CompletionData.User.Name);
            properties.Add(PropertyName_UserGivenName, e.CompletionData.User.GivenName);
            properties.Add(PropertyName_UserSurname, e.CompletionData.User.Surname);
        }

        if (_options.LogDeviceIpAddress)
        {
            properties.Add(PropertyName_BankIdUserDeviceIpAddress, e.CompletionData.Device.IpAddress);
        }

        if (_options.LogCertificateDates)
        {
            properties.Add(PropertyName_BankIdUserCertNotBefore, e.CompletionData.Cert.NotBefore);
            properties.Add(PropertyName_BankIdUserCertNotAfter, e.CompletionData.Cert.NotAfter);
        }

        var personalIdentityNumber = PersonalIdentityNumber.Parse(e.CompletionData.User.PersonalIdentityNumber);
        return Track(
            e,
            properties,
            personalIdentityNumber: personalIdentityNumber,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleCollectFailureEvent(BankIdCollectFailureEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef },
                { PropertyName_BankIdCollectHintCode, e.HintCode.ToString() }
            },
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleCollectErrorEvent(BankIdCollectErrorEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef }
            },
            exception: e.BankIdApiException,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    // BankID - Cancel

    public override Task HandleCancelSuccessEvent(BankIdCancelSuccessEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef }
            },
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    public override Task HandleCancelFailureEvent(BankIdCancelErrorEvent e)
    {
        return Track(
            e,
            new Dictionary<string, string>
            {
                { PropertyName_BankIdOrderRef, e.OrderRef }
            },
            exception: e.BankIdApiException,
            detectedDevice: e.DetectedUserDevice,
            loginOptions: e.BankIdOptions
        );
    }

    // Helpers

    private Task Track(BankIdEvent e, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null, PersonalIdentityNumber? personalIdentityNumber = null, Exception? exception = null, BankIdSupportedDevice? detectedDevice = null, BankIdLoginOptions? loginOptions = null)
    {
        var allProperties = properties == null ? new Dictionary<string, string>() : new Dictionary<string, string>(properties);
        var allMetrics = metrics == null ? new Dictionary<string, double>() : new Dictionary<string, double>(metrics);

        allProperties.Add(PropertyName_ProductName, e.ActiveLoginProductName);
        allProperties.Add(PropertyName_ProductVersion, e.ActiveLoginProductVersion);
        allProperties.Add(PropertyName_BankIdApiEnvironment, e.BankIdApiEnvironment);
        allProperties.Add(PropertyName_BankIdApiVersion, e.BankIdApiVersion);

        allProperties.Add(PropertyName_EventTypeName, e.EventTypeName);
        allProperties.Add(PropertyName_EventTypeId, e.EventTypeId.ToString("D"));
        allProperties.Add(PropertyName_EventSeverity, e.EventSeverity.ToString());

        if (loginOptions != null)
        {
            AddLoginOptionsProperties(allProperties, loginOptions);
        }

        if (personalIdentityNumber != null)
        {
            AddPersonalIdentityNumberProperties(allProperties, allMetrics, personalIdentityNumber);
        }

        if (detectedDevice != null)
        {
            AddUserDeviceProperties(allProperties, detectedDevice);
        }

        if (exception != null)
        {
            if (exception is BankIdApiException bankIdApiException)
            {
                allProperties.Add(PropertyName_BankIdErrorCode, bankIdApiException.ErrorCode.ToString());
                allProperties.Add(PropertyName_BankIdErrorDetails, bankIdApiException.ErrorDetails);
            }

            _telemetryClient.TrackException(exception, allProperties, metrics);
        }

        _telemetryClient.TrackEvent(e.EventTypeName, allProperties, allMetrics);

        return Task.CompletedTask;
    }

    private void AddPersonalIdentityNumberProperties(Dictionary<string, string> properties, Dictionary<string, double> metrics, PersonalIdentityNumber personalIdentityNumber)
    {
        if (_options.LogUserPersonalIdentityNumber)
        {
            properties.Add(PropertyName_UserSwedishPersonalIdentityNumber, personalIdentityNumber?.To12DigitString() ?? string.Empty);
        }

        if (_options.LogUserPersonalIdentityNumberHints)
        {
            properties.Add(PropertyName_UserDateOfBirthHint, personalIdentityNumber?.GetDateOfBirthHint().ToString("yyyy-MM-dd") ?? string.Empty);
            properties.Add(PropertyName_UserGenderHint, personalIdentityNumber?.GetGenderHint().ToString() ?? string.Empty);

            var ageHint = personalIdentityNumber?.GetAgeHint();
            if (ageHint != null)
            {
                metrics.Add(PropertyName_UserAgeHint, ageHint.Value);
            }
        }
    }

    private void AddUserDeviceProperties(Dictionary<string, string> properties, BankIdSupportedDevice userDevice)
    {
        if (_options.LogUserDevice)
        {
            properties.Add(PropertyName_UserDeviceBrowser, userDevice.DeviceBrowser.ToString());
            properties.Add(PropertyName_UserDeviceOs, userDevice.DeviceOs.ToString());
            properties.Add(PropertyName_UserDeviceType, userDevice.DeviceType.ToString());
            properties.Add(PropertyName_UserDeviceOsVersion, userDevice.DeviceOsVersion.ToString());
        }
    }

    private void AddLoginOptionsProperties(Dictionary<string, string> properties, BankIdLoginOptions loginOptions)
    {
        properties.Add(PropertyName_LoginOptionsLaunchType, GetLaunchType(loginOptions.SameDevice));
        properties.Add(PropertyName_LoginOptionsUseQrCode, GetBooleanProperty(!loginOptions.SameDevice));
    }

    private static string GetLaunchType(bool isSameDevice)
    {
        return isSameDevice ? "SameDevice" : "OtherDevice";
    }

    private static string GetBooleanProperty(bool property)
    {
        return property ? "True" : "False";
    }
}

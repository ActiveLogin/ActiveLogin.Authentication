using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.Requirements;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowService(
    IBankIdAppApiClient bankIdAppApiClient,
    IBankIdFlowSystemClock bankIdFlowSystemClock,
    IBankIdEventTrigger bankIdEventTrigger,
    IBankIdUserMessage bankIdUserMessage,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    IBankIdEndUserIpResolver bankIdEndUserIpResolver,
    IBankIdAuthRequestUserDataResolver bankIdAuthUserDataResolver,
    IBankIdAuthRequestRequirementsResolver bankIdAuthRequestRequirementsResolver,
    IBankIdQrCodeGenerator bankIdQrCodeGenerator,
    IBankIdLauncher bankIdLauncher,
    IBankIdCertificatePolicyResolver bankIdCertificatePolicyResolver,
    IBankIdEndUserDeviceDataResolverFactory bankIdEndUserDeviceDataResolverFactory,
    IBankIdRedirectUrlResolver bankIdRedirectUrlResolver
) : IBankIdFlowService
{
    private const int MaxRetryLoginAttempts = 5;

    public async Task<BankIdFlowInitializeResult> InitializeAuth(
        BankIdFlowOptions flowOptions,
        string authHandlerCallbackPath)
    {
        var detectedUserDevice = bankIdSupportedDeviceDetector.Detect();
        var response = await AuthAsync(flowOptions, detectedUserDevice, authHandlerCallbackPath);

        if (flowOptions.SameDevice)
        {
            var launchInfo = await bankIdLauncher.GetLaunchInfoAsync(new LaunchUrlRequest(authHandlerCallbackPath, response.AutoStartToken));
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<AuthResponse> AuthAsync(
        BankIdFlowOptions flowOptions,
        BankIdSupportedDevice detectedUserDevice,
        string authHandlerCallbackPath)
    {
        try
        {
            var request = await GetAuthRequest(flowOptions, authHandlerCallbackPath);
            var response = await bankIdAppApiClient.AuthAsync(request);
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));
            return response;
        }
        catch (BankIdApiException bankIdApiException)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private async Task<AuthRequest> GetAuthRequest(BankIdFlowOptions flowOptions, string authHandlerCallbackPath)
    {
        var resolvedRequirements = await bankIdAuthRequestRequirementsResolver.GetRequirementsAsync();
        var requiredPersonalIdentityNumber = resolvedRequirements.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;
        var certificatePolicies = resolvedRequirements.CertificatePolicies.Any()
            ? resolvedRequirements.CertificatePolicies
            : flowOptions.CertificatePolicies;

        var requestRequirement = new Requirement(
            GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice),
            resolvedRequirements.RequirePinCode ?? flowOptions.RequirePinCode,
            resolvedRequirements.RequireMrtd ?? flowOptions.RequireMrtd,
            requiredPersonalIdentityNumber?.To12DigitString(),
            resolvedRequirements.CardReader ?? flowOptions.CardReader
        );

        var authRequestContext = new BankIdAuthRequestContext(bankIdEndUserIpResolver.GetEndUserIp(), requestRequirement);
        var userData = await bankIdAuthUserDataResolver.GetUserDataAsync(authRequestContext);
        var (webDeviceData, appDeviceData) = GetDeviceData();

        return new AuthRequest(
            bankIdEndUserIpResolver.GetEndUserIp(),
            requirement: requestRequirement,
            userVisibleData: userData.UserVisibleData,
            userNonVisibleData: userData.UserNonVisibleData,
            userVisibleDataFormat: userData.UserVisibleDataFormat,
            returnUrl: await bankIdRedirectUrlResolver.GetRedirectUrl(BankIdTransactionType.Auth, authHandlerCallbackPath),
            returnRisk: flowOptions.ReturnRisk,
            web: webDeviceData,
            app: appDeviceData
        );
    }

    public async Task<BankIdFlowInitializeResult> InitializeSign(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, string callbackPath)
    {
        var detectedUserDevice = bankIdSupportedDeviceDetector.Detect();
        var response = await SignAsync(flowOptions, bankIdSignData, detectedUserDevice, callbackPath);

        if (flowOptions.SameDevice)
        {
            var launchInfo = await bankIdLauncher.GetLaunchInfoAsync(new LaunchUrlRequest(callbackPath, response.AutoStartToken));
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<SignResponse> SignAsync(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, BankIdSupportedDevice detectedUserDevice, string callbackPath)
    {
        try
        {
            var request = await GetSignRequest(flowOptions, bankIdSignData, callbackPath);
            var response = await bankIdAppApiClient.SignAsync(request);
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));
            return response;
        }
        catch (BankIdApiException bankIdApiException)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private async Task<SignRequest> GetSignRequest(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, string callbackPath)
    {
        var certificatePolicies = bankIdSignData.CertificatePolicies.Any()
            ? bankIdSignData.CertificatePolicies
            : flowOptions.CertificatePolicies;

        var requiredPersonalIdentityNumber = bankIdSignData.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;

        var requestRequirement = new Requirement(
            GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice),
            bankIdSignData.RequirePinCode ?? flowOptions.RequirePinCode,
            bankIdSignData.RequireMrtd ?? flowOptions.RequireMrtd,
            requiredPersonalIdentityNumber?.To12DigitString(),
            bankIdSignData.CardReader ?? flowOptions.CardReader
        );

        var (webDeviceData, appDeviceData) = GetDeviceData();

        return new SignRequest(
            bankIdEndUserIpResolver.GetEndUserIp(),
            bankIdSignData.UserVisibleData,
            userNonVisibleData: bankIdSignData.UserNonVisibleData,
            userVisibleDataFormat: bankIdSignData.UserVisibleDataFormat,
            requirement: requestRequirement,
            returnUrl: await bankIdRedirectUrlResolver.GetRedirectUrl(BankIdTransactionType.Sign, callbackPath),
            returnRisk: flowOptions.ReturnRisk,
            web: webDeviceData,
            app: appDeviceData
        );
    }

    private (DeviceDataWeb?, DeviceDataApp?) GetDeviceData()
    {
        var deviceDataResolver = bankIdEndUserDeviceDataResolverFactory.GetResolver();
        var deviceData = deviceDataResolver.GetDeviceData();
        return deviceDataResolver.DeviceType switch
        {
            BankIdEndUserDeviceType.Web => (deviceData as DeviceDataWeb, null),
            BankIdEndUserDeviceType.App => (null, deviceData as DeviceDataApp),
            _ => throw new InvalidOperationException($"Unknown device type: {deviceDataResolver.DeviceType}")
        };
    }

    public async Task<BankIdFlowInitializeResult> InitializePayment(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, string controllerInitUrl)
    {
        var detectedUserDevice = bankIdSupportedDeviceDetector.Detect();
        var response = await PaymentAsync(flowOptions, bankIdPaymentData, detectedUserDevice, controllerInitUrl);

        if (flowOptions.SameDevice)
        {
            var launchInfo = await bankIdLauncher.GetLaunchInfoAsync(new LaunchUrlRequest(controllerInitUrl, response.AutoStartToken));
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<PaymentResponse> PaymentAsync(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, BankIdSupportedDevice detectedUserDevice, string callbackPath)
    {
        try
        {
            var request = await GetPaymentRequest(flowOptions, bankIdPaymentData, callbackPath);
            var response = await bankIdAppApiClient.PaymentAsync(request);
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));
            return response;
        }
        catch (BankIdApiException bankIdApiException)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private async Task<PaymentRequest> GetPaymentRequest(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, string callbackPath)
    {
        var userVisibleTransaction = new UserVisibleTransaction(
            bankIdPaymentData.TransactionType.ToString(),
            new Recipient(bankIdPaymentData.RecipientName),
            bankIdPaymentData.Money,
            bankIdPaymentData.RiskWarning
        );

        var certificatePolicies = bankIdPaymentData.CertificatePolicies.Any()
            ? bankIdPaymentData.CertificatePolicies
            : flowOptions.CertificatePolicies;

        var requiredPersonalIdentityNumber = bankIdPaymentData.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;

        var requestRequirement = new Requirement(
            GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice),
            bankIdPaymentData.RequirePinCode ?? flowOptions.RequirePinCode,
            bankIdPaymentData.RequireMrtd ?? flowOptions.RequireMrtd,
            requiredPersonalIdentityNumber?.To12DigitString(),
            bankIdPaymentData.CardReader ?? flowOptions.CardReader
        );

        var (webDeviceData, appDeviceData) = GetDeviceData();

        var riskFlags = bankIdPaymentData.RiskFlags == null || !bankIdPaymentData.RiskFlags.Any()
            ? null
            : bankIdPaymentData.RiskFlags
                .Select(x => x.ToString())
                .ToList();

        return new PaymentRequest(
            bankIdEndUserIpResolver.GetEndUserIp(),
            userVisibleTransaction,
            userVisibleData: bankIdPaymentData.UserVisibleData,
            userNonVisibleData: bankIdPaymentData.UserNonVisibleData,
            userVisibleDataFormat: bankIdPaymentData.UserVisibleDataFormat,
            requirement: requestRequirement,
            returnRisk: bankIdPaymentData.ReturnRisk ?? flowOptions.ReturnRisk,
            returnUrl: await bankIdRedirectUrlResolver.GetRedirectUrl(BankIdTransactionType.Payment, callbackPath),
            riskFlags: riskFlags,
            app: appDeviceData,
            web: webDeviceData
        );
    }

    private List<string>? GetResolvedCertificatePolicies(List<BankIdCertificatePolicy> certificatePolicies, bool sameDevice)
    {
        if (certificatePolicies == null || !certificatePolicies.Any())
        {
            if (!sameDevice)
            {
                // Enforce mobile bank id for other device if no other policy is set
                certificatePolicies = [BankIdCertificatePolicy.MobileBankId];
            }
            else
            {
                return null;
            }
        }

        return certificatePolicies
            .Select(bankIdCertificatePolicyResolver.Resolve)
            .ToList();
    }

    public async Task<BankIdFlowCollectResult> Collect(string orderRef, int autoStartAttempts, BankIdFlowOptions flowOptions)
    {
        var detectedUserDevice = bankIdSupportedDeviceDetector.Detect();

        var collectResponse = await GetCollectResponse(orderRef, flowOptions, detectedUserDevice);
        var statusMessage = GetStatusMessage(collectResponse, flowOptions, detectedUserDevice);

        var collectStatus = collectResponse.GetCollectStatus();
        switch (collectStatus)
        {
            case CollectStatus.Pending:
            {
                await bankIdEventTrigger.TriggerAsync(new BankIdCollectPendingEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultPending(statusMessage);
            }
            case CollectStatus.Complete:
            {
                if (collectResponse.CompletionData == null)
                {
                    throw new InvalidOperationException("Missing CompletionData from BankID API");
                }

                await bankIdEventTrigger.TriggerAsync(new BankIdCollectCompletedEvent(collectResponse.OrderRef, collectResponse.CompletionData, detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultComplete(collectResponse.CompletionData);
            }
            case CollectStatus.Failed:
            {
                var hintCode = collectResponse.GetCollectHintCode();
                if (hintCode.Equals(CollectHintCode.StartFailed) && autoStartAttempts < MaxRetryLoginAttempts)
                {
                    return new BankIdFlowCollectResultRetry(statusMessage);
                }

                await bankIdEventTrigger.TriggerAsync(new BankIdCollectFailureEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultFailure(statusMessage);
            }
            default:
            {
                await bankIdEventTrigger.TriggerAsync(new BankIdCollectFailureEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultFailure(statusMessage);
            }
        }
    }

    private async Task<CollectResponse> GetCollectResponse(string orderRef, BankIdFlowOptions flowOptions, BankIdSupportedDevice detectedUserDevice)
    {
        try
        {
            return await bankIdAppApiClient.CollectAsync(orderRef);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdCollectErrorEvent(orderRef, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private string GetStatusMessage(CollectResponse collectResponse, BankIdFlowOptions unprotectedFlowOptions, BankIdSupportedDevice detectedDevice)
    {
        var accessedFromMobileDevice = detectedDevice.DeviceType == BankIdSupportedDeviceType.Mobile;
        var usingQrCode = !unprotectedFlowOptions.SameDevice;

        var messageShortName = bankIdUserMessage.GetMessageShortNameForCollectResponse(
            collectResponse.GetCollectStatus(),
            collectResponse.GetCollectHintCode(),
            accessedFromMobileDevice,
            usingQrCode
        );

        return bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);
    }

    public string GetQrCodeAsBase64(BankIdQrStartState qrStartState)
    {
        var elapsedTime = bankIdFlowSystemClock.UtcNow - qrStartState.QrStartTime;
        var elapsedTotalSeconds = (int)Math.Round(elapsedTime.TotalSeconds);

        var qrCodeContent = BankIdQrCodeContentGenerator.Generate(qrStartState.QrStartToken, qrStartState.QrStartSecret, elapsedTotalSeconds);

        return bankIdQrCodeGenerator.GenerateQrCodeAsBase64(qrCodeContent);
    }

    public async Task Cancel(string orderRef, BankIdFlowOptions flowOptions)
    {
        var detectedDevice = bankIdSupportedDeviceDetector.Detect();

        try
        {
            await bankIdAppApiClient.CancelAsync(orderRef);
            await bankIdEventTrigger.TriggerAsync(new BankIdCancelSuccessEvent(orderRef, detectedDevice, flowOptions));
        }
        catch (BankIdApiException exception)
        {
            // When we get exception in a cancellation request, chances
            // are that the orderRef has already been cancelled or we have
            // a network issue. We still want to provide the GUI with the
            // validated cancellation URL though.
            await bankIdEventTrigger.TriggerAsync(new BankIdCancelErrorEvent(orderRef, exception, detectedDevice, flowOptions));
        }
    }
}

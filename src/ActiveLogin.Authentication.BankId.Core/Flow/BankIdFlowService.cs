using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.Payment;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.Requirements;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowService : IBankIdFlowService
{
    private const int MaxRetryLoginAttempts = 5;

    private readonly IBankIdAppApiClient _bankIdAppApiClient;
    private readonly IBankIdFlowSystemClock _bankIdFlowSystemClock;
    private readonly IBankIdEventTrigger _bankIdEventTrigger;
    private readonly IBankIdUserMessage _bankIdUserMessage;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly IBankIdEndUserIpResolver _bankIdEndUserIpResolver;
    private readonly IBankIdAuthRequestUserDataResolver _bankIdAuthUserDataResolver;
    private readonly IBankIdAuthRequestRequirementsResolver _bankIdAuthRequestRequirementsResolver;
    private readonly IBankIdQrCodeGenerator _bankIdQrCodeGenerator;
    private readonly IBankIdLauncher _bankIdLauncher;
    private readonly IBankIdCertificatePolicyResolver _bankIdCertificatePolicyResolver;
    private readonly IBankIdEndUserDeviceDataResolverFactory _bankIdEndUserDeviceDataResolverFactory;

    public BankIdFlowService(
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
        IBankIdEndUserDeviceDataResolverFactory bankIdEndUserDeviceDataResolverFactory
    )
    {
        _bankIdAppApiClient = bankIdAppApiClient;
        _bankIdFlowSystemClock = bankIdFlowSystemClock;
        _bankIdEventTrigger = bankIdEventTrigger;
        _bankIdUserMessage = bankIdUserMessage;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _bankIdEndUserIpResolver = bankIdEndUserIpResolver;
        _bankIdAuthUserDataResolver = bankIdAuthUserDataResolver;
        _bankIdAuthRequestRequirementsResolver = bankIdAuthRequestRequirementsResolver;
        _bankIdQrCodeGenerator = bankIdQrCodeGenerator;
        _bankIdLauncher = bankIdLauncher;
        _bankIdCertificatePolicyResolver = bankIdCertificatePolicyResolver;
        _bankIdEndUserDeviceDataResolverFactory = bankIdEndUserDeviceDataResolverFactory;
    }

    public async Task<BankIdFlowInitializeResult> InitializeAuth(BankIdFlowOptions flowOptions, string returnRedirectUrl)
    {
        var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();
        var returnUrl = await GetReturnUrl(flowOptions, returnRedirectUrl);

        var response = await AuthAsync(flowOptions, detectedUserDevice, returnUrl);

        await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));

        if (flowOptions.SameDevice)
        {
            var launchInfo = await GetBankIdLaunchInfo(returnRedirectUrl, response.AutoStartToken);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                _bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<AuthResponse> AuthAsync(BankIdFlowOptions flowOptions, BankIdSupportedDevice detectedUserDevice, string? returnUrl)
    {
        try
        {
            var request = await GetAuthRequest(flowOptions, returnUrl);
            return await _bankIdAppApiClient.AuthAsync(request);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private async Task<AuthRequest> GetAuthRequest(BankIdFlowOptions flowOptions, string? returnUrl)
    {
        var endUserIp = _bankIdEndUserIpResolver.GetEndUserIp();
        var resolvedRequirements = await _bankIdAuthRequestRequirementsResolver.GetRequirementsAsync();
        var requiredPersonalIdentityNumber = resolvedRequirements.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;
        var requireMrtd = resolvedRequirements.RequireMrtd ?? flowOptions.RequireMrtd;
        var requirePinCode = resolvedRequirements.RequirePinCode ?? flowOptions.RequirePinCode;
        var certificatePolicies = resolvedRequirements.CertificatePolicies.Any() ? resolvedRequirements.CertificatePolicies : flowOptions.CertificatePolicies;
        var resolvedCertificatePolicies = GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice);

        var cardReader = resolvedRequirements.CardReader ?? flowOptions.CardReader;
        var requestRequirement = new Requirement(resolvedCertificatePolicies, requirePinCode, requireMrtd, requiredPersonalIdentityNumber?.To12DigitString(), cardReader);

        var returnRisk = flowOptions.ReturnRisk;

        var authRequestContext = new BankIdAuthRequestContext(endUserIp, requestRequirement);
        var userData = await _bankIdAuthUserDataResolver.GetUserDataAsync(authRequestContext);
        var (webDeviceData, appDeviceData) = GetDeviceData();

        return new AuthRequest(
            endUserIp,
            requirement: requestRequirement,
            userVisibleData: userData.UserVisibleData,
            userNonVisibleData: userData.UserNonVisibleData,
            userVisibleDataFormat: userData.UserVisibleDataFormat,
            returnUrl: returnUrl,
            returnRisk: returnRisk,
            web: webDeviceData,
            app: appDeviceData
        );
    }

    public async Task<BankIdFlowInitializeResult> InitializeSign(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, string returnRedirectUrl)
    {
        var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();
        var returnUrl = await GetReturnUrl(flowOptions, returnRedirectUrl);

        var response = await SignAsync(flowOptions, bankIdSignData, detectedUserDevice, returnUrl);

        await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));

        if (flowOptions.SameDevice)
        {
            var launchInfo = await GetBankIdLaunchInfo(returnRedirectUrl, response.AutoStartToken);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                _bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<SignResponse> SignAsync(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, BankIdSupportedDevice detectedUserDevice, string? returnUrl)
    {
        try
        {
            var request = GetSignRequest(flowOptions, bankIdSignData, returnUrl);
            return await _bankIdAppApiClient.SignAsync(request);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private SignRequest GetSignRequest(BankIdFlowOptions flowOptions, BankIdSignData bankIdSignData, string? returnUrl)
    {
        var endUserIp = _bankIdEndUserIpResolver.GetEndUserIp();

        var certificatePolicies = bankIdSignData.CertificatePolicies.Any()
            ? bankIdSignData.CertificatePolicies
            : flowOptions.CertificatePolicies;
        var resolvedCertificatePolicies = GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice);

        var requiredPersonalIdentityNumber = bankIdSignData.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;
        var requireMrtd = bankIdSignData.RequireMrtd ?? flowOptions.RequireMrtd;
        var requirePinCode = bankIdSignData.RequirePinCode ?? flowOptions.RequirePinCode;
        var cardReader = bankIdSignData.CardReader ?? flowOptions.CardReader;
        var requestRequirement = new Requirement(resolvedCertificatePolicies, requirePinCode, requireMrtd, requiredPersonalIdentityNumber?.To12DigitString(), cardReader);

        var returnRisk = flowOptions.ReturnRisk;

        var (webDeviceData, appDeviceData) = GetDeviceData();

        return new SignRequest(
            endUserIp,
            bankIdSignData.UserVisibleData,
            userNonVisibleData: bankIdSignData.UserNonVisibleData,
            userVisibleDataFormat: bankIdSignData.UserVisibleDataFormat,
            requirement: requestRequirement,
            returnUrl: returnUrl,
            returnRisk: returnRisk,
            web: webDeviceData,
            app: appDeviceData
        );
    }

    private (DeviceDataWeb?, DeviceDataApp?) GetDeviceData()
    {
        var deviceDataResolver = _bankIdEndUserDeviceDataResolverFactory.GetResolver();
        var deviceData = deviceDataResolver.GetDeviceData();
        return deviceDataResolver.DeviceType switch
        {
            BankIdEndUserDeviceType.Web => (deviceData as DeviceDataWeb, null),
            BankIdEndUserDeviceType.App => (null, deviceData as DeviceDataApp),
            _ => throw new InvalidOperationException($"Unknown device type: {deviceDataResolver.DeviceType}")
        };
    }

    public async Task<BankIdFlowInitializeResult> InitializePayment(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, string returnRedirectUrl)
    {
        var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();
        var returnUrl = await GetReturnUrl(flowOptions, returnRedirectUrl);

        var response = await PaymentAsync(flowOptions, bankIdPaymentData, detectedUserDevice, returnUrl);

        await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeSuccessEvent(personalIdentityNumber: null, response.OrderRef, detectedUserDevice, flowOptions));

        if (flowOptions.SameDevice)
        {
            var launchInfo = await GetBankIdLaunchInfo(returnRedirectUrl, response.AutoStartToken);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                _bankIdFlowSystemClock.UtcNow,
                response.QrStartToken,
                response.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeResult(response, detectedUserDevice, new BankIdFlowInitializeLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<PaymentResponse> PaymentAsync(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, BankIdSupportedDevice detectedUserDevice, string? returnUrl)
    {
        try
        {
            var request = GetPaymentRequest(flowOptions, bankIdPaymentData, returnUrl);
            return await _bankIdAppApiClient.PaymentAsync(request);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdInitializeErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private PaymentRequest GetPaymentRequest(BankIdFlowOptions flowOptions, BankIdPaymentData bankIdPaymentData, string? returnUrl)
    {
        var endUserIp = _bankIdEndUserIpResolver.GetEndUserIp();

        var transactionType = bankIdPaymentData.TransactionType;
        var recipientName = bankIdPaymentData.RecipientName;
        var recipient = new Recipient(recipientName);
        var money = bankIdPaymentData.Money;
        var riskWarning = bankIdPaymentData.RiskWarning;
        var userVisibleTransaction = new UserVisibleTransaction(transactionType.ToString(), recipient, money, riskWarning);

        var certificatePolicies = bankIdPaymentData.CertificatePolicies.Any() ? bankIdPaymentData.CertificatePolicies : flowOptions.CertificatePolicies;
        var resolvedCertificatePolicies = GetResolvedCertificatePolicies(certificatePolicies, flowOptions.SameDevice);
        var cardReader = bankIdPaymentData.CardReader ?? flowOptions.CardReader;

        var requiredPersonalIdentityNumber = bankIdPaymentData.RequiredPersonalIdentityNumber ?? flowOptions.RequiredPersonalIdentityNumber;
        var requireMrtd = bankIdPaymentData.RequireMrtd ?? flowOptions.RequireMrtd;
        var requirePinCode = bankIdPaymentData.RequirePinCode ?? flowOptions.RequirePinCode;
        var requestRequirement = new Requirement(resolvedCertificatePolicies, requirePinCode, requireMrtd, requiredPersonalIdentityNumber?.To12DigitString(), cardReader);

        var returnRisk = bankIdPaymentData.ReturnRisk;

        var riskFlags = GetResolvedRiskFlags(bankIdPaymentData.RiskFlags);

        var (webDeviceData, appDeviceData) = GetDeviceData();

        return new PaymentRequest(
            endUserIp,
            userVisibleTransaction,
            userVisibleData: bankIdPaymentData.UserVisibleData,
            userNonVisibleData: bankIdPaymentData.UserNonVisibleData,
            userVisibleDataFormat: bankIdPaymentData.UserVisibleDataFormat,
            requirement: requestRequirement,
            returnRisk: returnRisk,
            returnUrl: returnUrl,
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

        return certificatePolicies.Select(x => _bankIdCertificatePolicyResolver.Resolve(x)).ToList();
    }

    private List<string>? GetResolvedRiskFlags(IEnumerable<RiskFlags>? riskFlags)
    {
        if (riskFlags == null || !riskFlags.Any()) return null;

        return riskFlags.Select(x => x.ToString()).ToList();
    }

    private async Task<string?> GetReturnUrl(BankIdFlowOptions flowOptions, string returnRedirectUrl)
    {
        if (!flowOptions.SameDevice) return null;

        var launchUrlRequest = new LaunchUrlRequest(returnRedirectUrl, "");
        var launchInfo = await _bankIdLauncher.GetLaunchInfoAsync(launchUrlRequest);

        return launchInfo.ReturnUrl;
    }

    private Task<BankIdLaunchInfo> GetBankIdLaunchInfo(string redirectUrl, string autoStartToken)
    {
        var launchUrlRequest = new LaunchUrlRequest(redirectUrl, autoStartToken);

        return _bankIdLauncher.GetLaunchInfoAsync(launchUrlRequest);
    }

    public async Task<BankIdFlowCollectResult> Collect(string orderRef, int autoStartAttempts, BankIdFlowOptions flowOptions)
    {
        var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();

        var collectResponse = await GetCollectResponse(orderRef, flowOptions, detectedUserDevice);
        var statusMessage = GetStatusMessage(collectResponse, flowOptions, detectedUserDevice);

        var collectStatus = collectResponse.GetCollectStatus();
        switch (collectStatus)
        {
            case CollectStatus.Pending:
            {
                await _bankIdEventTrigger.TriggerAsync(new BankIdCollectPendingEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultPending(statusMessage);
            }
            case CollectStatus.Complete:
            {
                if (collectResponse.CompletionData == null)
                {
                    throw new InvalidOperationException("Missing CompletionData from BankID API");
                }

                await _bankIdEventTrigger.TriggerAsync(new BankIdCollectCompletedEvent(collectResponse.OrderRef, collectResponse.CompletionData, detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultComplete(collectResponse.CompletionData);
            }
            case CollectStatus.Failed:
            {
                var hintCode = collectResponse.GetCollectHintCode();
                if (hintCode.Equals(CollectHintCode.StartFailed) && autoStartAttempts < MaxRetryLoginAttempts)
                {
                    return new BankIdFlowCollectResultRetry(statusMessage);
                }

                await _bankIdEventTrigger.TriggerAsync(new BankIdCollectFailureEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultFailure(statusMessage);
            }
            default:
            {
                await _bankIdEventTrigger.TriggerAsync(new BankIdCollectFailureEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedUserDevice, flowOptions));
                return new BankIdFlowCollectResultFailure(statusMessage);
            }
        }
    }

    private async Task<CollectResponse> GetCollectResponse(string orderRef, BankIdFlowOptions flowOptions, BankIdSupportedDevice detectedUserDevice)
    {
        try
        {
            return await _bankIdAppApiClient.CollectAsync(orderRef);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdCollectErrorEvent(orderRef, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private string GetStatusMessage(CollectResponse collectResponse, BankIdFlowOptions unprotectedFlowOptions, BankIdSupportedDevice detectedDevice)
    {
        var accessedFromMobileDevice = detectedDevice.DeviceType == BankIdSupportedDeviceType.Mobile;
        var usingQrCode = !unprotectedFlowOptions.SameDevice;

        var messageShortName = _bankIdUserMessage.GetMessageShortNameForCollectResponse(
            collectResponse.GetCollectStatus(),
            collectResponse.GetCollectHintCode(),
            accessedFromMobileDevice,
            usingQrCode
        );
        var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

        return statusMessage;
    }

    public string GetQrCodeAsBase64(BankIdQrStartState qrStartState)
    {
        var elapsedTime = _bankIdFlowSystemClock.UtcNow - qrStartState.QrStartTime;
        var elapsedTotalSeconds = (int)Math.Round(elapsedTime.TotalSeconds);

        var qrCodeContent = BankIdQrCodeContentGenerator.Generate(qrStartState.QrStartToken, qrStartState.QrStartSecret, elapsedTotalSeconds);
        var qrCode = _bankIdQrCodeGenerator.GenerateQrCodeAsBase64(qrCodeContent);

        return qrCode;
    }

    public async Task Cancel(string orderRef, BankIdFlowOptions flowOptions)
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

        try
        {
            await _bankIdAppApiClient.CancelAsync(orderRef);
            await _bankIdEventTrigger.TriggerAsync(new BankIdCancelSuccessEvent(orderRef, detectedDevice, flowOptions));
        }
        catch (BankIdApiException exception)
        {
            // When we get exception in a cancellation request, chances
            // are that the orderRef has already been cancelled or we have
            // a network issue. We still want to provide the GUI with the
            // validated cancellation URL though.
            await _bankIdEventTrigger.TriggerAsync(new BankIdCancelErrorEvent(orderRef, exception, detectedDevice, flowOptions));
        }
    }
}

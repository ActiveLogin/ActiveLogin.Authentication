using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

namespace ActiveLogin.Authentication.BankId.Core.Flow;

public class BankIdFlowService : IBankIdFlowService
{
    private const int MaxRetryLoginAttempts = 5;

    private readonly IBankIdApiClient _bankIdApiClient;
    private readonly IBankIdFlowSystemClock _bankIdFlowSystemClock;
    private readonly IBankIdEventTrigger _bankIdEventTrigger;
    private readonly IBankIdUserMessage _bankIdUserMessage;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly IBankIdEndUserIpResolver _bankIdEndUserIpResolver;
    private readonly IBankIdAuthRequestUserDataResolver _bankIdAuthUserDataResolver;
    private readonly IBankIdQrCodeGenerator _bankIdQrCodeGenerator;
    private readonly IBankIdLauncher _bankIdLauncher;

    public BankIdFlowService(
        IBankIdApiClient bankIdApiClient,
        IBankIdFlowSystemClock bankIdFlowSystemClock,
        IBankIdEventTrigger bankIdEventTrigger,
        IBankIdUserMessage bankIdUserMessage,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
        IBankIdEndUserIpResolver bankIdEndUserIpResolver,
        IBankIdAuthRequestUserDataResolver bankIdAuthUserDataResolver,
        IBankIdQrCodeGenerator bankIdQrCodeGenerator,
        IBankIdLauncher bankIdLauncher
    )
    {
        _bankIdApiClient = bankIdApiClient;
        _bankIdFlowSystemClock = bankIdFlowSystemClock;
        _bankIdEventTrigger = bankIdEventTrigger;
        _bankIdUserMessage = bankIdUserMessage;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _bankIdEndUserIpResolver = bankIdEndUserIpResolver;
        _bankIdAuthUserDataResolver = bankIdAuthUserDataResolver;
        _bankIdQrCodeGenerator = bankIdQrCodeGenerator;
        _bankIdLauncher = bankIdLauncher;
    }

    public async Task<BankIdFlowInitializeAuthResult> InitializeAuth(BankIdFlowOptions flowOptions, string returnRedirectUrl)
    {
        var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();
        var authResponse = await GetAuthResponse(flowOptions, detectedUserDevice);

        await _bankIdEventTrigger.TriggerAsync(new BankIdAuthSuccessEvent(personalIdentityNumber: null, authResponse.OrderRef, detectedUserDevice, flowOptions));

        if (flowOptions.SameDevice)
        {
            var launchInfo = GetBankIdLaunchInfo(returnRedirectUrl, authResponse);
            return new BankIdFlowInitializeAuthResult(authResponse, detectedUserDevice, new BankIdFlowInitializeAuthLaunchTypeSameDevice(launchInfo));
        }
        else
        {
            var qrStartState = new BankIdQrStartState(
                _bankIdFlowSystemClock.UtcNow,
                authResponse.QrStartToken,
                authResponse.QrStartSecret
            );

            var qrCodeAsBase64 = GetQrCodeAsBase64(qrStartState);
            return new BankIdFlowInitializeAuthResult(authResponse, detectedUserDevice, new BankIdFlowInitializeAuthLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
        }
    }

    private async Task<AuthResponse> GetAuthResponse(BankIdFlowOptions flowOptions, BankIdSupportedDevice detectedUserDevice)
    {
        try
        {
            var authRequest = await GetAuthRequest(flowOptions);
            return await _bankIdApiClient.AuthAsync(authRequest);
        }
        catch (BankIdApiException bankIdApiException)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdAuthErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, flowOptions));
            throw;
        }
    }

    private async Task<AuthRequest> GetAuthRequest(BankIdFlowOptions flowOptions)
    {
        var endUserIp = _bankIdEndUserIpResolver.GetEndUserIp();
        var certificatePolicies = flowOptions.CertificatePolicies.Any() ? flowOptions.CertificatePolicies : null;

        var authRequestRequirement = new Requirement(certificatePolicies, tokenStartRequired: true, flowOptions.AllowBiometric);

        var authRequestContext = new BankIdAuthRequestContext(endUserIp, authRequestRequirement);
        var userData = await _bankIdAuthUserDataResolver.GetUserDataAsync(authRequestContext);

        return new AuthRequest(endUserIp, null, authRequestRequirement, userData.UserVisibleData, userData.UserNonVisibleData, userData.UserVisibleDataFormat);
    }

    private BankIdLaunchInfo GetBankIdLaunchInfo(string redirectUrl, AuthResponse authResponse)
    {
        var launchUrlRequest = new LaunchUrlRequest(redirectUrl, authResponse.AutoStartToken);

        return _bankIdLauncher.GetLaunchInfo(launchUrlRequest);
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
            return await _bankIdApiClient.CollectAsync(orderRef);
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
            authPersonalIdentityNumberProvided: false,
            accessedFromMobileDevice,
            usingQrCode);
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
            await _bankIdApiClient.CancelAsync(orderRef);
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

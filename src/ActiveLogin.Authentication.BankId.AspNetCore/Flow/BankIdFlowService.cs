using System;
using System.Linq;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Flow
{
    public abstract class InitializeAuthFlowLaunchType
    {
    }

    public class InitializeAuthFlowLaunchTypeSameDevice : InitializeAuthFlowLaunchType
    {
        public BankIdLaunchInfo BankIdLaunchInfo { get; init; }

        public InitializeAuthFlowLaunchTypeSameDevice(BankIdLaunchInfo bankIdLaunchInfo)
        {
            BankIdLaunchInfo = bankIdLaunchInfo;
        }
    }

    public class InitializeAuthFlowLaunchTypeOtherDevice : InitializeAuthFlowLaunchType
    {
        public InitializeAuthFlowLaunchTypeOtherDevice(BankIdQrStartState qrStartState, string qrCodeBase64Encoded)
        {
            QrStartState = qrStartState;
            QrCodeBase64Encoded = qrCodeBase64Encoded;
        }

        public BankIdQrStartState QrStartState { get; init; }

        public string QrCodeBase64Encoded { get; init; }
    }


    public class InitializeAuthFlowResult
    {
        public InitializeAuthFlowResult(AuthResponse bankIdAuthResponse, BankIdSupportedDevice detectedUserDevice, InitializeAuthFlowLaunchType launchType)
        {
            BankIdAuthResponse = bankIdAuthResponse;
            DetectedUserDevice = detectedUserDevice;
            LaunchType = launchType;
        }

        public AuthResponse BankIdAuthResponse { get; init; }

        public BankIdSupportedDevice DetectedUserDevice { get; init; }

        public InitializeAuthFlowLaunchType LaunchType { get; init; }
    }

    public class InitializeAuthFlowSameDeviceResult
    {
    }

    internal class BankIdFlowService : IBankIdFlowService
    {
        private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
        private readonly IBankIdEndUserIpResolver _bankIdEndUserIpResolver;
        private readonly IBankIdAuthRequestUserDataResolver _bankIdAuthUserDataResolver;
        private readonly IBankIdApiClient _bankIdApiClient;
        private readonly IBankIdEventTrigger _bankIdEventTrigger;
        private readonly IBankIdQrCodeContentGenerator _bankIdQrCodeContentGenerator;
        private readonly IBankIdQrCodeGenerator _qrCodeGenerator;
        private readonly IBankIdLauncher _bankIdLauncher;

        public BankIdFlowService(
            IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
            IBankIdEndUserIpResolver bankIdEndUserIpResolver,
            IBankIdAuthRequestUserDataResolver bankIdAuthUserDataResolver,
            IBankIdApiClient bankIdApiClient,
            IBankIdEventTrigger bankIdEventTrigger,
            IBankIdQrCodeContentGenerator bankIdQrCodeContentGenerator,
            IBankIdQrCodeGenerator qrCodeGenerator,
            IBankIdLauncher bankIdLauncher)
        {
            _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
            _bankIdEndUserIpResolver = bankIdEndUserIpResolver;
            _bankIdAuthUserDataResolver = bankIdAuthUserDataResolver;
            _bankIdApiClient = bankIdApiClient;
            _bankIdEventTrigger = bankIdEventTrigger;
            _bankIdQrCodeContentGenerator = bankIdQrCodeContentGenerator;
            _qrCodeGenerator = qrCodeGenerator;
            _bankIdLauncher = bankIdLauncher;
        }

        public async Task<InitializeAuthFlowResult> InitializeAuth(BankIdLoginOptions loginOptions, string returnRedirectUrl)
        {
            var detectedUserDevice = _bankIdSupportedDeviceDetector.Detect();
            AuthResponse authResponse;
            try
            {
                var authRequest = await GetAuthRequest(loginOptions);
                authResponse = await _bankIdApiClient.AuthAsync(authRequest);
            }
            catch (BankIdApiException bankIdApiException)
            {
                await _bankIdEventTrigger.TriggerAsync(new BankIdAuthErrorEvent(personalIdentityNumber: null, bankIdApiException, detectedUserDevice, loginOptions));
                throw;
            }

            await _bankIdEventTrigger.TriggerAsync(new BankIdAuthSuccessEvent(personalIdentityNumber: null, authResponse.OrderRef, detectedUserDevice, loginOptions));

            if (loginOptions.SameDevice)
            {
                var launchInfo = GetBankIdLaunchInfo(returnRedirectUrl, authResponse);
                return new InitializeAuthFlowResult(authResponse, detectedUserDevice, new InitializeAuthFlowLaunchTypeSameDevice(launchInfo));
            }
            else
            {
                var qrStartState = new BankIdQrStartState(
                    DateTimeOffset.UtcNow,
                    authResponse.QrStartToken,
                    authResponse.QrStartSecret
                );

                var qrCodeAsBase64 = GetQrCode(qrStartState);
                return new InitializeAuthFlowResult(authResponse, detectedUserDevice, new InitializeAuthFlowLaunchTypeOtherDevice(qrStartState, qrCodeAsBase64));
            }
        }

        private async Task<AuthRequest> GetAuthRequest(BankIdLoginOptions loginOptions)
        {
            var endUserIp = _bankIdEndUserIpResolver.GetEndUserIp();
            var certificatePolicies = loginOptions.CertificatePolicies.Any() ? loginOptions.CertificatePolicies : null;

            var authRequestRequirement = new Requirement(certificatePolicies, tokenStartRequired: true, loginOptions.AllowBiometric);

            var authRequestContext = new BankIdAuthRequestContext(endUserIp, authRequestRequirement);
            var userData = await _bankIdAuthUserDataResolver.GetUserDataAsync(authRequestContext);

            return new AuthRequest(endUserIp, null, authRequestRequirement, userData.UserVisibleData, userData.UserNonVisibleData, userData.UserVisibleDataFormat);
        }

        private BankIdLaunchInfo GetBankIdLaunchInfo(string redirectUrl, AuthResponse authResponse)
        {
            var launchUrlRequest = new LaunchUrlRequest(redirectUrl, authResponse.AutoStartToken);

            return _bankIdLauncher.GetLaunchInfo(launchUrlRequest);
        }

        public string GetQrCode(BankIdQrStartState qrStartState)
        {
            var elapsedTime = DateTimeOffset.UtcNow - qrStartState.QrStartTime;
            var elapsedTotalSeconds = (int)Math.Round(elapsedTime.TotalSeconds);

            var qrCodeContent = _bankIdQrCodeContentGenerator.Generate(qrStartState.QrStartToken, qrStartState.QrStartSecret, elapsedTotalSeconds);
            var qrCode = _qrCodeGenerator.GenerateQrCodeAsBase64(qrCodeContent);

            return qrCode;
        }
    }
}

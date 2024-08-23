using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.Risk;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserData;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Moq;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.Flow;

public class BankIdFlowService_Tests
{
    private readonly Mock<IBankIdAppApiClient> _bankIdAppApiClient;
    private readonly Mock<IBankIdFlowSystemClock> _bankIdFlowSystemClock;
    private readonly Mock<IBankIdEventTrigger> _bankIdEventTrigger;
    private readonly Mock<IBankIdUserMessage> _bankIdUserMessage;
    private readonly Mock<IBankIdUserMessageLocalizer> _bankIdUserMessageLocalizer;
    private readonly Mock<IBankIdSupportedDeviceDetector> _bankIdSupportedDeviceDetector;
    private readonly Mock<IBankIdEndUserIpResolver> _bankIdEndUserIpResolver;
    private readonly Mock<IBankIdAuthRequestUserDataResolver> _bankIdAuthUserDataResolver;
    private readonly Mock<IBankIdQrCodeGenerator> _bankIdQrCodeGenerator;
    private readonly Mock<IBankIdLauncher> _bankIdLauncher;
    private readonly Mock<IBankIdCertificatePolicyResolver> _bankIdCertificatePolicyResolver;

    public BankIdFlowService_Tests()
    {
        _bankIdAppApiClient = new Mock<IBankIdAppApiClient>();
        _bankIdFlowSystemClock = new Mock<IBankIdFlowSystemClock>();
        _bankIdEventTrigger = new Mock<IBankIdEventTrigger>();
        _bankIdUserMessage = new Mock<IBankIdUserMessage>();
        _bankIdUserMessageLocalizer = new Mock<IBankIdUserMessageLocalizer>();
        _bankIdSupportedDeviceDetector = new Mock<IBankIdSupportedDeviceDetector>();
        _bankIdEndUserIpResolver = new Mock<IBankIdEndUserIpResolver>();
        _bankIdAuthUserDataResolver = new Mock<IBankIdAuthRequestUserDataResolver>();
        _bankIdQrCodeGenerator = new Mock<IBankIdQrCodeGenerator>();
        _bankIdLauncher = new Mock<IBankIdLauncher>();
        _bankIdCertificatePolicyResolver = new Mock<IBankIdCertificatePolicyResolver>();

        _bankIdSupportedDeviceDetector
            .Setup(x => x.Detect())
            .Returns(new BankIdSupportedDevice(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari, BankIdSupportedDeviceOsVersion.Empty));

        _bankIdEndUserIpResolver
            .Setup(x => x.GetEndUserIp())
            .Returns("192.168.0.1");
    }

    [Fact]
    public async Task BankIdLauncher_Should_Deny_SameDevice_With_Mismatching_IpAddress()
    {
        var collectResponse = new CollectResponse("orderRef", "Complete", "Started",
            new CompletionData(
                new User("personalNumber", "name", "givenName", "surname"),
                new Device("192.168.0.2", "uhi"),
                "",
                new StepUp(false),
                "",
                "",
                ""
            ));

        _bankIdAppApiClient
            .Setup(x => x.CollectAsync(It.IsAny<CollectRequest>()))
            .Returns(Task.FromResult(collectResponse));

        var bankIdFlowService = new BankIdFlowService(
            _bankIdAppApiClient.Object,
            _bankIdFlowSystemClock.Object,
            _bankIdEventTrigger.Object,
            _bankIdUserMessage.Object,
            _bankIdUserMessageLocalizer.Object,
            _bankIdSupportedDeviceDetector.Object,
            _bankIdEndUserIpResolver.Object,
            _bankIdAuthUserDataResolver.Object,
            _bankIdQrCodeGenerator.Object,
            _bankIdLauncher.Object,
            _bankIdCertificatePolicyResolver.Object
        );

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var collectResult = await bankIdFlowService.Collect(
                "orderRef",
                0,
                new BankIdFlowOptions(
                    certificatePolicies: [],
                    allowedRiskLevel: Risk.BankIdAllowedRiskLevel.Low,
                    sameDevice: true,
                    requirePinCode: false,
                    requireMrtd: false,
                    requiredPersonalIdentityNumber: null
                )
            );
        });
    }

    [Fact]
    public async Task BankIdLauncher_Should_Allow_SameDevice_With_Mismatching_IpAddress()
    {
        var collectResponse = new CollectResponse("orderRef", "Complete", "Started",
            new CompletionData(
                new User("personalNumber", "name", "givenName", "surname"),
                new Device("192.168.0.1", "uhi"),
                "",
                new StepUp(false),
                "",
                "",
                ""
            ));

        _bankIdAppApiClient
            .Setup(x => x.CollectAsync(It.IsAny<CollectRequest>()))
            .Returns(Task.FromResult(collectResponse));

        var bankIdFlowService = new BankIdFlowService(
            _bankIdAppApiClient.Object,
            _bankIdFlowSystemClock.Object,
            _bankIdEventTrigger.Object,
            _bankIdUserMessage.Object,
            _bankIdUserMessageLocalizer.Object,
            _bankIdSupportedDeviceDetector.Object,
            _bankIdEndUserIpResolver.Object,
            _bankIdAuthUserDataResolver.Object,
            _bankIdQrCodeGenerator.Object,
            _bankIdLauncher.Object,
            _bankIdCertificatePolicyResolver.Object
        );


        var collectResult = await bankIdFlowService.Collect(
            "orderRef",
            0,
            new BankIdFlowOptions(
                certificatePolicies: [],
                allowedRiskLevel: BankIdAllowedRiskLevel.Low,
                sameDevice: true,
                requirePinCode: false,
                requireMrtd: false,
                requiredPersonalIdentityNumber: null
            )
        );

        Assert.NotNull(collectResult);
    }
}

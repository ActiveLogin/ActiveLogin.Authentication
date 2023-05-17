using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.UserMessage;

public class BankIdRecommendedUserMessage_CollectResponse_Tests
{
    private readonly BankIdRecommendedUserMessage _bankIdRecommendedUserMessage;

    public BankIdRecommendedUserMessage_CollectResponse_Tests()
    {
        _bankIdRecommendedUserMessage = new BankIdRecommendedUserMessage();
    }

    [Theory]
    [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
    [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.Started, MessageShortName.RFA15A)]
    [InlineData(CollectHintCode.UserMrtd, MessageShortName.RFA23)]
    [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_PendingOrder_FromPersonalComputer_AutomaticallyStartBankIdApp(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Pending;
        var accessedFromMobileDevice = false;
        var usingQrCode = false;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.UserMrtd, MessageShortName.RFA23)]
    [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_PendingOrder_FromPersonalComputer_UseQRCode(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Pending;
        var accessedFromMobileDevice = false;
        var usingQrCode = true;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
    [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.Started, MessageShortName.RFA15B)]
    [InlineData(CollectHintCode.UserMrtd, MessageShortName.RFA23)]
    [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_PendingOrder_FromMobileDevice_AutomaticallyStartBankIdApp(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Pending;
        var accessedFromMobileDevice = true;
        var usingQrCode = false;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
    [InlineData(CollectHintCode.UserMrtd, MessageShortName.RFA23)]
    [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_PendingOrder_FromMobileDevice_UseQRCode(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Pending;
        var accessedFromMobileDevice = true;
        var usingQrCode = true;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
    [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
    [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
    [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
    [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17A)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA22)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_FailedOrder_FromPersonalComputer_AutomaticallyStartBankIdApp(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Failed;
        var accessedFromMobileDevice = false;
        var usingQrCode = false;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
    [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
    [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
    [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
    [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17B)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA22)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_FailedOrder_FromPersonalComputer_UseQRCode(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Failed;
        var accessedFromMobileDevice = false;
        var usingQrCode = true;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
    [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
    [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
    [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
    [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17A)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA22)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_FailedOrder_FromMobileDevice_AutomaticallyStartBankIdApp(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Failed;
        var accessedFromMobileDevice = true;
        var usingQrCode = false;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
    [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
    [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
    [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
    [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17B)]
    [InlineData(CollectHintCode.Unknown, MessageShortName.RFA22)]
    public void GetMessageShortNameForCollectResponse_ShouldReturnRecommendedMessageForCollectResponse_FailedOrder_FromMobileDevice_UseQRCode(CollectHintCode collectHintCode, MessageShortName expected)
    {
        var collectStatus = CollectStatus.Failed;
        var accessedFromMobileDevice = true;
        var usingQrCode = true;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetMessageShortNameForCollectResponse_ShouldReturn_RFA22_When_Unknown()
    {
        var accessedFromMobileDevice = false;
        var usingQrCode = false;

        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(CollectStatus.Unknown, CollectHintCode.Unknown, accessedFromMobileDevice, usingQrCode);

        Assert.Equal(MessageShortName.RFA22, result);
    }
}

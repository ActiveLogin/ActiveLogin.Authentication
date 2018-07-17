using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.UserMessage
{
    public class BankIdRecommendedUserMessage_CollectResponse_Tests
    {
        private readonly BankIdRecommendedUserMessage _bankIdRecommendedUserMessage;

        public BankIdRecommendedUserMessage_CollectResponse_Tests()
        {
            _bankIdRecommendedUserMessage = new BankIdRecommendedUserMessage();
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA15A)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
        public void GetMessageShortNameForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_BankID_AutomaticallyStartBankIDAPP(CollectHintCode collectHintCode, MessageShortName expected)
        {
            var collectStatus = CollectStatus.Pending;
            var authPersonalIdentityNumberProvided = false;
            var accessedFromMobileDevice = false;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA14A)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
        public void GetMessageShortNameForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_BankID_ManuallyStartBankIDApp(CollectHintCode collectHintCode, MessageShortName expected)
        {
            var collectStatus = CollectStatus.Pending;
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = false;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA15B)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
        public void GetMessageShortNameForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_MobileBankID_AutomaticallyStartBankIDAPP(CollectHintCode collectHintCode, MessageShortName expected)
        {
            var collectStatus = CollectStatus.Pending;
            var authPersonalIdentityNumberProvided = false;
            var accessedFromMobileDevice = true;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA14B)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
        public void GetMessageShortNameForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_MobileBankID_ManuallyStartBankIDApp(CollectHintCode collectHintCode, MessageShortName expected)
        {
            var collectStatus = CollectStatus.Pending;
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = true;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
        [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
        [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
        [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
        [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA21)]
        public void GetMessageShortNameForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_FailedOrder(CollectHintCode collectHintCode, MessageShortName expected)
        {
            var collectStatus = CollectStatus.Pending;
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = false;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(collectStatus, collectHintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetMessageShortNameForCollectResponse_ShouldReturn_RFA22_When_Unknown()
        {
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = false;

            var result = _bankIdRecommendedUserMessage.GetMessageShortNameForCollectResponse(CollectStatus.Unknown, CollectHintCode.Unknown, authPersonalIdentityNumberProvided, accessedFromMobileDevice);

            Assert.Equal(MessageShortName.RFA22, result);
        }
    }
}

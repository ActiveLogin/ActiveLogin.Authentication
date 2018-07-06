using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models
{
    public class RecommendedUserMessageTests
    {
        
        [Fact]
        public void Create_ShouldCreateRecommendedUserMessage()
        {
            var result = RecommendedUserMessage.Create(MessageShortName.RFA1);
            Assert.IsType<RecommendedUserMessage>(result);
        }

        [Theory]
        [InlineData(MessageShortName.RFA1)]
        [InlineData(MessageShortName.RFA2)]
        [InlineData(MessageShortName.RFA14A)]
        public void ShortName_ShouldReturnShortName(MessageShortName shortName)
        {
            var message = RecommendedUserMessage.Create(shortName);
            var result = message.ShortName;
            Assert.Equal(shortName, result);
        }

        [Theory]
        [InlineData(MessageShortName.RFA1, "Starta BankID-appen")]
        [InlineData(MessageShortName.RFA2, "Du har inte BankID-appen installerad. Kontakta din internetbank.")]
        [InlineData(MessageShortName.RFA21, "Inloggning eller signering pågår.")]
        public void SwedishText_ShouldReturnSwedishText(MessageShortName shortName, string expected)
        {
            var message = RecommendedUserMessage.Create(shortName);
            var result = message.SwedishText;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EnglishText_ShouldReturnEnglishText()
        {
            const string text = "Start your BankID app.";
            var message = RecommendedUserMessage.Create(MessageShortName.RFA1);
            var result = message.EnglishText;
            Assert.Equal(text, result);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA15A)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.Unknown)]
        public void GetMessageForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_BankID_AutomaticallyStartBankIDAPP(CollectHintCode hintCode, MessageShortName expected)
        {
            const CollectStatus status = CollectStatus.Pending;
            const BankIdType type = BankIdType.BankId;
            const bool startAutomatically = true;
            var collectResponse = new CollectResponse(status, hintCode);

            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, startAutomatically, type);

            Assert.Equal(expected, result.ShortName);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA14A)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.Unknown)]
        public void GetMessageForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_BankID_ManuallyStartBankIDApp(CollectHintCode hintCode, MessageShortName expected)
        {
            const CollectStatus status = CollectStatus.Pending;
            const BankIdType type = BankIdType.BankId;
            const bool startAutomatically = false;
            var collectResponse = new CollectResponse(status, hintCode);
            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, startAutomatically, type);

            Assert.Equal(expected, result.ShortName);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA13)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA15B)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.Unknown)]
        public void GetMessageForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_MobileBankID_AutomaticallyStartBankIDAPP(CollectHintCode hintCode, MessageShortName expected)
        {
            const CollectStatus status = CollectStatus.Pending;
            const BankIdType type = BankIdType.MobileBankId;
            const bool startAutomatically = true;
            var collectResponse = new CollectResponse(status, hintCode);
            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, startAutomatically, type);

            Assert.Equal(expected, result.ShortName);
        }

        [Theory]
        [InlineData(CollectHintCode.OutstandingTransaction, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.NoClient, MessageShortName.RFA1)]
        [InlineData(CollectHintCode.Started, MessageShortName.RFA14B)]
        [InlineData(CollectHintCode.UserSign, MessageShortName.RFA9)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.Unknown)]
        public void GetMessageForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_PendingOrder_MobileBankID_ManuallyStartBankIDApp(CollectHintCode hintCode, MessageShortName expected)
        {
            const CollectStatus status = CollectStatus.Pending;
            const BankIdType type = BankIdType.MobileBankId;
            const bool startAutomatically = false;
            var collectResponse = new CollectResponse(status, hintCode);
            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, startAutomatically, type);

            Assert.Equal(expected, result.ShortName);
        }
        
        [Theory]
        [InlineData(CollectHintCode.ExpiredTransaction, MessageShortName.RFA8)]
        [InlineData(CollectHintCode.CertificateErr, MessageShortName.RFA16)]
        [InlineData(CollectHintCode.UserCancel, MessageShortName.RFA6)]
        [InlineData(CollectHintCode.Cancelled, MessageShortName.RFA3)]
        [InlineData(CollectHintCode.StartFailed, MessageShortName.RFA17)]
        [InlineData(CollectHintCode.Unknown, MessageShortName.RFA22)]
        public void GetMessageForCollectResponse_ShouldReturnReccomendedMessageForCollectResponse_FailedOrder(CollectHintCode hintCode, MessageShortName expected)
        {
            const CollectStatus status = CollectStatus.Failed;
            const bool autoStartBankIdApp = false;
            const BankIdType type = BankIdType.BankId;
            var collectResponse = new CollectResponse(status, hintCode);
            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, autoStartBankIdApp, type);

            Assert.Equal(expected, result.ShortName);
        }

        [Theory]
        [InlineData(CollectStatus.Pending, CollectHintCode.CertificateErr)]
        [InlineData(CollectStatus.Unknown, CollectHintCode.Unknown)]
        public void GetMessageForCollectResponse_ShouldReturnEmptyMessage_UnknownCollectResponse(CollectStatus status, CollectHintCode hintCode)
        {
            const bool autoStartBankIdApp = false;
            const BankIdType type = BankIdType.BankId;
            var collectResponse = new CollectResponse(status, hintCode);
            var result = RecommendedUserMessage.GetMessageForCollectResponse(collectResponse, autoStartBankIdApp, type);

            Assert.NotNull(result);
            Assert.Equal("", result.SwedishText);
            Assert.Equal("", result.EnglishText);
        }

        [Fact]
        public void GetMessageByShortName_ShouldReturnMessage()
        {
            var result = RecommendedUserMessage.GetMessageByShortName(MessageShortName.RFA13);
            Assert.Equal(MessageShortName.RFA13, result.ShortName);
        }
    }
}

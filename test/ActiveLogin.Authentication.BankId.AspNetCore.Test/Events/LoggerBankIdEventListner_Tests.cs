using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Events
{
    public class LoggerBankIdEventListner_Tests
    {
        private readonly Mock<ILogger<LoggerBankIdEventListner>> _loggerMock;
        private const string personalIdentityNumber = "990807-2391";
        private const string orderRef = "BE96E8B4-3BCC-43CE-9315-469095FB6A2B";

        public LoggerBankIdEventListner_Tests()
        {
            _loggerMock = new Mock<ILogger<LoggerBankIdEventListner>>();
        }

        [Fact]
        public void HandleBankIdAuthenticationTicketCreatedEvent_Should_Log_Event()
        {
            // Arrange
            var loggerBankIdEventLister = new LoggerBankIdEventListner(_loggerMock.Object);

            // Act
            loggerBankIdEventLister.HandleBankIdAuthenticationTicketCreatedEvent(new BankIdAuthenticationTicketCreatedEvent(personalIdentityNumber));

            // Assert
            Assert.Equal(2, _loggerMock.Invocations.Count);
            Assert.Equal(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
            Assert.Equal("BankID authentication ticket created", _loggerMock.Invocations[0].Arguments[2].ToString());
            Assert.Equal(LogLevel.Trace, _loggerMock.Invocations[1].Arguments[0]);
            Assert.Equal($"BankID authentication ticket created for PersonalIdentityNumber '{personalIdentityNumber}'", _loggerMock.Invocations[1].Arguments[2].ToString());
        }

        [Fact]
        public void HandleBankIdCollectPendingEvent_Should_Log_Event()
        {
            // Arrange
            const CollectHintCode collectHintCode = CollectHintCode.Started;
            var loggerBankIdEventLister = new LoggerBankIdEventListner(_loggerMock.Object);

            // Act
            loggerBankIdEventLister.HandleBankIdCollectPendingEvent(new BankIdCollectPendingEvent(orderRef, collectHintCode));

            // Assert
            Assert.Single(_loggerMock.Invocations);
            Assert.Equal(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
            Assert.Equal($"BankID collect is pending for OrderRef '{orderRef}' with HintCode '{collectHintCode}'", _loggerMock.Invocations[0].Arguments[2].ToString());
        }
    }
}

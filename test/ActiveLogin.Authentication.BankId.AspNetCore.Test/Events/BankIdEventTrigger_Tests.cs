using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using Xunit;
using Moq;
using Moq.Protected;
using System.Collections.Generic;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Events
{
    public class BankIdEventTrigger_Tests
    {
        private readonly Mock<IBankIdEventListener> _bankIdEventListnerMock;
        private const string personalIdentityNumber = "990807-2391";

        public BankIdEventTrigger_Tests()
        {
            _bankIdEventListnerMock = new Mock<IBankIdEventListener>();
        }

        [Fact]
        public void TriggerAsync_Should_Send_Event_To_Attached_Listners()
        {
            // Arrange
            var bankIdEventTrigger = new BankIdEventTrigger(new List<IBankIdEventListener>() { _bankIdEventListnerMock.Object });

            // Act
            bankIdEventTrigger.TriggerAsync(new BankIdAuthenticationTicketCreatedEvent(personalIdentityNumber));
            
            // Assert
            Assert.Single(_bankIdEventListnerMock.Invocations);
            Assert.Equal("HandleAsync", _bankIdEventListnerMock.Invocations[0].Method.Name);
            Assert.IsType<BankIdAuthenticationTicketCreatedEvent>( _bankIdEventListnerMock.Invocations[0].Arguments[0]);
            var bankIdEvent = (BankIdAuthenticationTicketCreatedEvent) _bankIdEventListnerMock.Invocations[0].Arguments[0];
            Assert.Equal(personalIdentityNumber, bankIdEvent.PersonalIdentityNumber);
        }
    }
}

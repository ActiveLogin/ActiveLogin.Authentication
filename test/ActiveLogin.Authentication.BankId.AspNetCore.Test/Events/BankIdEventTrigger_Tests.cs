using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using Xunit;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Events
{
    public class BankIdEventTrigger_Tests
    {
        private readonly Mock<IBankIdEventListener> _bankIdEventListnerMock;

        public BankIdEventTrigger_Tests()
        {
            _bankIdEventListnerMock = new Mock<IBankIdEventListener>();
        }

        [Fact]
        public async void TriggerAsync_Should_Send_Event_To_Attached_Listners()
        {
            // Arrange
            var bankIdEventTrigger = new BankIdEventTrigger(new List<IBankIdEventListener>() { _bankIdEventListnerMock.Object });

            // Act
            await bankIdEventTrigger.TriggerAsync(new TestBankIdEvent());

            // Assert
            Assert.Single(_bankIdEventListnerMock.Invocations);
            Assert.Equal("HandleAsync", _bankIdEventListnerMock.Invocations[0].Method.Name);
            Assert.IsType<TestBankIdEvent>(_bankIdEventListnerMock.Invocations[0].Arguments[0]);
        }
    }
}

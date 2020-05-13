using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Events.Infrastructure
{
    public class BankIdDebugEventListener_Tests
    {
        [Fact]
        public async Task Serializes_Event_As_Json()
        {
            // Arrange
            var logger = new Mock<ILogger<BankIdDebugEventListener>>();
            var debugEventListener = new BankIdDebugEventListener(logger.Object);
            var testEvent = new TestBankIdEvent(999, "TestEvent", EventSeverity.Information);

            // Act

            await debugEventListener.HandleAsync(testEvent);

            // Assert
            var invocation = logger.Invocations[0];

            var eventId = (EventId)invocation.Arguments[1];
            Assert.Equal("TestEvent", eventId.Name);
            Assert.Equal(999, eventId.Id);

            var json = invocation.Arguments[2].ToString();
            Assert.Contains("\"eventTypeId\": 999", json);
            Assert.Contains("\"eventTypeName\": \"TestEvent\"", json);
            Assert.Contains("\"eventSeverity\": \"Information\"", json);
        }
    }
}

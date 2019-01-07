using Moq;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.TestHelpers
{
    public static class MockExtensions
    {
        public static TArgument GetFirstArgumentOfFirstInvocation<TMock, TArgument>(this Mock<TMock> mock) where TMock : class where TArgument : class
        {
            Assert.NotEmpty(mock.Invocations);
            var invocation = mock.Invocations[0];

            Assert.NotEmpty(invocation.Arguments);
            var argument = invocation.Arguments[0] as TArgument;

            return argument;
        }
    }
}

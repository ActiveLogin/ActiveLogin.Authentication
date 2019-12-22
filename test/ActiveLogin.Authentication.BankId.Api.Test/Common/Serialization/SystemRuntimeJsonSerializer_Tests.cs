using System.Runtime.Serialization;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Common.Serialization
{
    public class SystemRuntimeJsonSerializer_Tests
    {
        private const string SerializedObject = "{\"errorCode\":\"code\",\"details\":\"more\"}";

        [DataContract]
        internal class Error
        {
            public static Error Empty = new Error(string.Empty, string.Empty);

            public Error(string errorCode, string details)
            {
                ErrorCode = errorCode;
                Details = details;
            }

            [DataMember(Name = "errorCode")]
            public string ErrorCode { get; private set; }

            [DataMember(Name = "details")]
            public string Details { get; private set; }
        }

        [Fact]
        public async Task SerializeAsync__ShouldReturnString()
        {
            // Arrange
            var model = new Error("code", "more");

            // Act
            var json = await SystemRuntimeJsonSerializer.SerializeAsync(model);

            // Assert
            Assert.Equal(SerializedObject, json);
        }

        [Fact]
        public async Task DeserializeAsync__ShouldReturnErrorObject()
        {
            // Act
            var error = await SystemRuntimeJsonSerializer.DeserializeAsync<Error>(SerializedObject);

            // Assert
            Assert.NotNull(error);
            Assert.Equal("code", error.ErrorCode);
            Assert.Equal("more", error.Details);
        }
    }
}

using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test
{
    public class Authentication_Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Authentication_Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Authentication_UI_Should_Be_Accessible_Even_When_Site_Uses_Auth()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder => { builder.UseEnvironment(EnvironmentName.Production); })
                .CreateClient();

            // Act
            var response = await client.GetAsync("/api/debug/login?username=testuser&role=admin");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}

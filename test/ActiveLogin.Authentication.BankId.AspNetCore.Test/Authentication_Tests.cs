using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test
{
    public class Authentication_Tests
    {


        [Fact]
        public async Task Authentication_UI_Should_Be_Accessible_Even_When_Site_Uses_Auth()
        {
            //// Arrange
            //var client = _factory.WithWebHostBuilder()
            //    .CreateClient();

            //// Act
            //var response = await client.GetAsync("/api/debug/login?username=testuser&role=admin");

            //// Assert
            //Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}

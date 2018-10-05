using System;
using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test
{
    public class BankIdDevelopmentApiClient_Tests
    {
        private readonly BankIdDevelopmentApiClient _bankIdClient;

        public BankIdDevelopmentApiClient_Tests()
        {
            _bankIdClient = new BankIdDevelopmentApiClient
            {
                Delay = TimeSpan.Zero
            };
        }

        [Fact]
        public async void AuthAsync__WithSamePersonalIdentityNumber_AtTheSameTime__ShouldThrow()
        {
            await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391"));
            await Assert.ThrowsAsync<BankIdApiException>(() => _bankIdClient.AuthAsync(new AuthRequest("1.1.1.2", "199908072391")));
        }

        [Fact]
        public async void AuthAsync__WithSamePersonalIdentityNumber_OneAtTheTime__ShouldBeAllowed()
        {
            AuthResponse authResponse = await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391"));
            CollectResponse response;
            do
            {
                response = await _bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef));
            } while (response.Status != CollectStatus.Complete);

            response = null;

            authResponse = await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.2", "199908072391"));
            do
            {
                response = await _bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef));
            } while (response.Status != CollectStatus.Complete);

            Assert.True(true, "Did not throw");
        }
    }
}

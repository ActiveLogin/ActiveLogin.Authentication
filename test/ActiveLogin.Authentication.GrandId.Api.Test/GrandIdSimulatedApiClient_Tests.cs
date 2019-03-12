using System;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.GrandId.Api.Test
{
    public class GrandIdSimulatedApiClient_Tests
    {
        public GrandIdSimulatedApiClient_Tests()
        {
            _grandIdClient = new GrandIdSimulatedApiClient
            {
                Delay = TimeSpan.Zero
            };
        }

        private readonly GrandIdSimulatedApiClient _grandIdClient;

        [Fact]
        public async void BankIdFederatedLoginAsync_WithSamePersonalIdentityNumber_AtTheSameTime__ShouldThrow()
        {
            // Arange

            // Act
            await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392"));

            // Assert
            await Assert.ThrowsAsync<GrandIdApiException>(() => _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392")));
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithSamePersonalIdentityNumber_OneAtTheTime__ShouldBeAllowed()
        {
            // Arange

            // Act
            BankIdFederatedLoginResponse firstLoginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392"));
            await _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(firstLoginResponse.SessionId));


            BankIdFederatedLoginResponse secondLoginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392"));
            await _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(secondLoginResponse.SessionId));

            // Assert
            Assert.True(true, "Did not throw");
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithDefaultValuesInConstructor__ShouldReturnPersonInfo()
        {
            // Arange
            var grandIdClient = new GrandIdSimulatedApiClient("gn", "sn", "201801012392")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            BankIdFederatedLoginResponse loginResponse = await grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/"));
            BankIdGetSessionResponse sessionResponse = await grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(loginResponse.SessionId));

            // Assert
            Assert.Equal("201801012392", sessionResponse.Username);
            Assert.Equal("201801012392", sessionResponse.UserAttributes.PersonalIdentityNumber);
            Assert.Equal("gn", sessionResponse.UserAttributes.GivenName);
            Assert.Equal("sn", sessionResponse.UserAttributes.Surname);
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithSpecifiedPin_InBankIdLoginRequest__ShouldReturnPersonInfo_WithPin()
        {
            // Arange
            var grandIdClient = new GrandIdSimulatedApiClient("x", "x", "x")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            BankIdFederatedLoginResponse loginResponse =  await grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392"));
            BankIdGetSessionResponse sessionResponse = await grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(loginResponse.SessionId));

            // Assert
            Assert.Equal("201801012392", sessionResponse.Username);
            Assert.Equal("201801012392", sessionResponse.UserAttributes.PersonalIdentityNumber);
        }

        [Fact]
        public async void LogoutAsync_CancelsTheCollectFlow()
        {
            // Arange

            // Act
            BankIdFederatedLoginResponse loginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/"));
            await _grandIdClient.LogoutAsync(new LogoutRequest(loginResponse.SessionId));

            // Assert
            await Assert.ThrowsAsync<GrandIdApiException>(() => _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(loginResponse.SessionId)));
        }
    }
}

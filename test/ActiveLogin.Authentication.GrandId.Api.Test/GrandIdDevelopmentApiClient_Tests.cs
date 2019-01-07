using System;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.GrandId.Api.Test
{
    public class GrandIdDevelopmentApiClient_Tests
    {
        private readonly GrandIdDevelopmentApiClient _grandIdClient;

        public GrandIdDevelopmentApiClient_Tests()
        {
            _grandIdClient = new GrandIdDevelopmentApiClient
            {
                Delay = TimeSpan.Zero
            };
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithSamePersonalIdentityNumber_AtTheSameTime__ShouldThrow()
        {
            // Arange

            // Act
            await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/", "201801012392"));

            // Assert
            await Assert.ThrowsAsync<GrandIdApiException>(() => _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/", "201801012392")));
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithSamePersonalIdentityNumber_OneAtTheTime__ShouldBeAllowed()
        {
            // Arange

            // Act
            var firstLoginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/", "201801012392"));
            await _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", firstLoginResponse.SessionId));


            var secondLoginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/", "201801012392"));
            await _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", secondLoginResponse.SessionId));

            // Assert
            Assert.True(true, "Did not throw");
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithDefaultValuesInConstructor__ShouldReturnPersonInfo()
        {
            // Arange
            var grandIdClient = new GrandIdDevelopmentApiClient("gn", "sn", "201801012392")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var loginResponse = await grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/"));
            var sessionResponse = await grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", loginResponse.SessionId));

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
            var grandIdClient = new GrandIdDevelopmentApiClient("x", "x", "x")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var loginResponse = await grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/", "201801012392"));
            var sessionResponse = await grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", loginResponse.SessionId));

            // Assert
            Assert.Equal("201801012392", sessionResponse.Username);
            Assert.Equal("201801012392", sessionResponse.UserAttributes.PersonalIdentityNumber);
        }

        [Fact]
        public async void FederatedDirectLoginAsync__ShouldReturnPersonInfo()
        {
            // Arange
            var grandIdClient = new GrandIdDevelopmentApiClient("gn", "sn", "x", "1")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var loginResponse = await grandIdClient.FederatedDirectLoginAsync(new FederatedDirectLoginRequest("x", "u", "p"));

            // Assert
            Assert.Equal("u", loginResponse.Username);
            Assert.Equal("u", loginResponse.UserAttributes.SameAccountName);

            Assert.Equal("gn", loginResponse.UserAttributes.GivenName);
            Assert.Equal("sn", loginResponse.UserAttributes.Surname);
            Assert.Equal("1", loginResponse.UserAttributes.MobilePhone);
            Assert.Equal("Software Developer", loginResponse.UserAttributes.Title);
        }

        [Fact]
        public async void LogoutAsync_CancelsTheCollectFlow()
        {
            // Arange

            // Act
            var loginResponse = await _grandIdClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("x", "https://c/"));
            await _grandIdClient.LogoutAsync(new LogoutRequest(loginResponse.SessionId));

            // Assert
            await Assert.ThrowsAsync<GrandIdApiException>(() => _grandIdClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", loginResponse.SessionId)));
        }
    }
}

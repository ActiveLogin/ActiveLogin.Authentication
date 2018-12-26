using System;
using System.Collections.Generic;
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
        public async void AuthAsync_WithSamePersonalIdentityNumber_AtTheSameTime__ShouldThrow()
        {
            // Arange

            // Act
            await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392"));

            // Assert
            await Assert.ThrowsAsync<BankIdApiException>(() => _bankIdClient.AuthAsync(new AuthRequest("1.1.1.2", "201801012392")));
        }

        [Fact]
        public async void AuthAsync_WithSamePersonalIdentityNumber_OneAtTheTime__ShouldBeAllowed()
        {
            // Arange

            // Act
            var firstAuthResponse = await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392"));
            CollectResponse firstCollectResponse;
            do
            {
                firstCollectResponse = await _bankIdClient.CollectAsync(new CollectRequest(firstAuthResponse.OrderRef));
            } while (firstCollectResponse.GetCollectStatus() != CollectStatus.Complete);


            var secondAuthResponse = await _bankIdClient.AuthAsync(new AuthRequest("1.1.1.2", "201801012392"));
            CollectResponse secondCollectResponse;
            do
            {
                secondCollectResponse = await _bankIdClient.CollectAsync(new CollectRequest(secondAuthResponse.OrderRef));
            } while (secondCollectResponse.GetCollectStatus() != CollectStatus.Complete);

            // Assert
            Assert.True(true, "Did not throw");
        }

        [Fact]
        public async void CollectAsync_WithDefaultValuesInConstructor__ShouldReturnPersonInfo()
        {
            // Arange
            var bankIdClient = new BankIdDevelopmentApiClient("gn", "sn", "n", "201801012392")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var authResponse = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));
            CollectResponse collectResponse;
            do
            {
                collectResponse = await bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef));
            } while (collectResponse.GetCollectStatus() != CollectStatus.Complete);

            // Assert
            Assert.Equal("gn", collectResponse.CompletionData.User.GivenName);
            Assert.Equal("sn", collectResponse.CompletionData.User.Surname);
            Assert.Equal("n", collectResponse.CompletionData.User.Name);
            Assert.Equal("201801012392", collectResponse.CompletionData.User.PersonalIdentityNumber);
        }

        [Fact]
        public async void CollectAsync_WithSpecifiedEndUserIp_AndPin_InAuthRequest__ShouldReturnPersonInfo_WithEndUserIp_AndPin()
        {
            // Arange
            var bankIdClient = new BankIdDevelopmentApiClient("x", "x", "x", "x")
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var authResponse = await bankIdClient.AuthAsync(new AuthRequest("2.2.2.2", "201801012392"));
            CollectResponse collectResponse;
            do
            {
                collectResponse = await bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef));
            } while (collectResponse.GetCollectStatus() != CollectStatus.Complete);
            
            // Assert
            Assert.Equal("2.2.2.2", collectResponse.CompletionData.Device.IpAddress);
            Assert.Equal("201801012392", collectResponse.CompletionData.User.PersonalIdentityNumber);
        }

        [Fact]
        public async void CancelAsync_CancelsTheCollectFlow()
        {
            // Arange
            var statuses = new List<BankIdDevelopmentApiClient.CollectState>
            {
                new BankIdDevelopmentApiClient.CollectState(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
                new BankIdDevelopmentApiClient.CollectState(CollectStatus.Pending, CollectHintCode.Started),
                new BankIdDevelopmentApiClient.CollectState(CollectStatus.Complete, CollectHintCode.UserSign)
            };
            var bankIdClient = new BankIdDevelopmentApiClient(statuses)
            {
                Delay = TimeSpan.Zero
            };

            // Act
            var authResponse = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));
            await bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef));
            await bankIdClient.CancelAsync(new CancelRequest(authResponse.OrderRef));

            // Assert
            await Assert.ThrowsAsync<BankIdApiException>(() => bankIdClient.CollectAsync(new CollectRequest(authResponse.OrderRef)));
        }
    }
}

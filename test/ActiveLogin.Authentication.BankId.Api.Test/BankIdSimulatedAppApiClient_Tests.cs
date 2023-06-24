using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test;

public class BankIdSimulatedAppApiClient_Tests
{
    private readonly BankIdSimulatedAppApiClient _bankIdAppClient;

    public BankIdSimulatedAppApiClient_Tests()
    {
        _bankIdAppClient = new BankIdSimulatedAppApiClient
        {
            Delay = TimeSpan.Zero
        };
    }

    [Fact]
    public async Task AuthAsync_MultipleAuthAtTheSameTime__ShouldThrow()
    {
        // Arange

        // Act
        await _bankIdAppClient.AuthAsync(new AuthRequest("1.1.1.1"));

        // Assert
        await Assert.ThrowsAsync<BankIdApiException>(() => _bankIdAppClient.AuthAsync(new AuthRequest("1.1.1.2")));
    }

    [Fact]
    public async Task AuthAsync_OneAuthAtTheTime__ShouldBeAllowed()
    {
        // Arange

        // Act
        var firstAuthResponse = await _bankIdAppClient.AuthAsync(new AuthRequest("1.1.1.1"));
        CollectResponse firstCollectResponse;
        do
        {
            firstCollectResponse = await _bankIdAppClient.CollectAsync(new CollectRequest(firstAuthResponse.OrderRef));
        } while (firstCollectResponse.GetCollectStatus() != CollectStatus.Complete);

        var secondAuthResponse = await _bankIdAppClient.AuthAsync(new AuthRequest("1.1.1.2"));
        CollectResponse secondCollectResponse;
        do
        {
            secondCollectResponse = await _bankIdAppClient.CollectAsync(new CollectRequest(secondAuthResponse.OrderRef));
        } while (secondCollectResponse.GetCollectStatus() != CollectStatus.Complete);

        // Assert
    }

    [Fact]
    public async Task CollectAsync_WithDefaultValuesInConstructor__ShouldReturnPersonInfo()
    {
        // Arange
        var bankIdClient = new BankIdSimulatedAppApiClient("gn", "sn", "n", "201801012392")
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
    public async Task CollectAsync_WithSpecifiedEndUserIp_InAuthRequest__ShouldReturnPersonInfo_WithEndUserIp_AndPin()
    {
        // Arange
        var bankIdClient = new BankIdSimulatedAppApiClient("x", "x", "x", "201801012392")
        {
            Delay = TimeSpan.Zero
        };

        // Act
        var authResponse = await bankIdClient.AuthAsync(new AuthRequest("2.2.2.2"));
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
    public async Task CancelAsync_CancelsTheCollectFlow()
    {
        // Arange
        var statuses = new List<BankIdSimulatedAppApiClient.CollectState>
        {
            new BankIdSimulatedAppApiClient.CollectState(CollectStatus.Pending, CollectHintCode.OutstandingTransaction),
            new BankIdSimulatedAppApiClient.CollectState(CollectStatus.Pending, CollectHintCode.Started),
            new BankIdSimulatedAppApiClient.CollectState(CollectStatus.Complete, CollectHintCode.UserSign)
        };
        var bankIdClient = new BankIdSimulatedAppApiClient(statuses)
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

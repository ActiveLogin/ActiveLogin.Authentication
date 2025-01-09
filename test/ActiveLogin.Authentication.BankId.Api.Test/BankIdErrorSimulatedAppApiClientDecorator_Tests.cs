#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api.Models;

using AutoFixture;

using Moq;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test;

public class BankIdErrorSimulatedApiClientDecoratorTests
{
    private readonly Fixture _fixture = new Fixture();
    private readonly Mock<IBankIdAppApiClient> _apiClientMock = new();
    

    private BankIdErrorSimulatedApiClientDecorator CreateSut(double errorRate, Dictionary<ErrorCode, string> errors, bool varyErrorType)
    {
        var apiClient = _apiClientMock.Object;
        return new BankIdErrorSimulatedApiClientDecorator(apiClient, errorRate, errors, varyErrorType);
    }

    private Dictionary<ErrorCode, string> GenerateErrors(params ErrorCode[] errorCodes)
    {
        if (errorCodes.Length == 0)
        {
            errorCodes = [ErrorCode.AlreadyInProgress, ErrorCode.Maintenance, ErrorCode.RequestTimeout, ErrorCode.InternalError];
        }

        return errorCodes.ToDictionary(
            errorCode => errorCode,
            errorCode => errorCode.ToString());
    }

    [Fact]
    public async Task Returns_Same_Error()
    {
        // Arrange
        var sut = CreateSut(
            1,
            GenerateErrors(ErrorCode.InternalError, ErrorCode.Maintenance),
            false);

        var exceptions = new List<BankIdApiException>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            exceptions.Add(await Assert.ThrowsAsync<BankIdApiException>(() =>
                sut.AuthAsync(_fixture.Build<AuthRequest>().Create())));
        }

        // Assert - That the error is the same for each sut call
        var errorCode = exceptions.First().ErrorCode;
        Assert.All(exceptions, exception =>
        {
            Assert.Equal(errorCode, exception.ErrorCode);
        });

    }

    [Fact]
    public async Task Returns_Error_Variations()
    {
        // Arrange
        var sut = CreateSut(
            1,
            GenerateErrors(ErrorCode.InternalError, ErrorCode.Maintenance),
            true);

        var exceptions = new List<BankIdApiException>();

        // Act
        for (var i = 0; i < 50; i++)
        {
            exceptions.Add(await Assert.ThrowsAsync<BankIdApiException>(() =>
                sut.AuthAsync(_fixture.Build<AuthRequest>().Create())));
        }

        // Assert - That there are multiple error codes in the exceptions
        var first = exceptions.First().ErrorCode;
        Assert.False(exceptions.All(x => x.ErrorCode == exceptions.First().ErrorCode), "Errors do not vary.");
    }

    [Fact]
    public async Task Returns_Only_Errors_From_List()
    {
        // Arrange
        var allowedErrorCodes = new[] { ErrorCode.InternalError, ErrorCode.Maintenance, ErrorCode.TooManyRequests };
        var sut = CreateSut(
            1,
            GenerateErrors(allowedErrorCodes),
            true);

        var exceptions = new List<BankIdApiException>();

        // Act
        for (var i = 0; i < 50; i++)
        {
            exceptions.Add(await Assert.ThrowsAsync<BankIdApiException>(() =>
                sut.AuthAsync(_fixture.Build<AuthRequest>().Create())));
        }

        // Assert - That there are only allowed error codes in the exceptions
        Assert.All(exceptions, exception =>
        {
            Assert.Contains(exception.ErrorCode, allowedErrorCodes);
        });

    }

    [Fact]
    public async Task Can_Call_All_Decorated_Client_Methods()
    {
        // Arrange
        var sut = CreateSut(0, new Dictionary<ErrorCode, string>(), false);

        // Setup AutoFixture to use RP as CallInitiator
        var phoneAuthRequest =
            new PhoneAuthRequest(_fixture.Create<string>(), CallInitiator.RP, null, null, null, null);

        var phoneSignRequest =
                       new PhoneSignRequest(_fixture.Create<string>(), CallInitiator.RP, _fixture.Create<string>(), null, null, null);

        // Act
        await sut.AuthAsync(_fixture.Build<AuthRequest>().Create());
        await sut.SignAsync(_fixture.Build<SignRequest>().Create());
        await sut.PhoneAuthAsync(phoneAuthRequest);
        await sut.PhoneSignAsync(phoneSignRequest);
        await sut.CollectAsync(_fixture.Build<CollectRequest>().Create());
        await sut.CancelAsync(_fixture.Build<CancelRequest>().Create());

        // Assert
        _apiClientMock.Verify(x => x.AuthAsync(It.IsAny<AuthRequest>()), Times.Once);
        _apiClientMock.Verify(x => x.SignAsync(It.IsAny<SignRequest>()), Times.Once);
        _apiClientMock.Verify(x => x.PhoneAuthAsync(It.IsAny<PhoneAuthRequest>()), Times.Once);
        _apiClientMock.Verify(x => x.PhoneSignAsync(It.IsAny<PhoneSignRequest>()), Times.Once);
        _apiClientMock.Verify(x => x.CollectAsync(It.IsAny<CollectRequest>()), Times.Once);
        _apiClientMock.Verify(x => x.CancelAsync(It.IsAny<CancelRequest>()), Times.Once);
    }

    [Fact]
    public async Task Decorator_Has_Correct_Error_Rate()
    {
        // Arrange
        var requestCount = 1000;
        var errorRate = 0.5;
        var sut = CreateSut(errorRate, GenerateErrors(), true);

        // Act
        var errorCount = 0;
        for (var i = 0; i < requestCount; i++)
        {
            try
            {
                await sut.AuthAsync(_fixture.Build<AuthRequest>().Create());
            }
            catch (BankIdApiException)
            {
                errorCount++;
            }
        }

        // Assert
        var actualErrorRate = (double)errorCount / requestCount;
        Assert.True(Math.Abs(actualErrorRate - errorRate) < 0.1, $"Error rate was {actualErrorRate} but expected {errorRate}");
    }

    [Fact]
    public void Throws_Exception_When_Error_Rate_Is_Invalid()
    {
        // Arrange
        var errorRate = 1.1;
        // Act
        var ex = Assert.Throws<ArgumentException>(() => CreateSut(errorRate, GenerateErrors(), true));
        // Assert
        Assert.Equal("Error rate must be between 0 and 1.", ex.Message);
    }
    
}

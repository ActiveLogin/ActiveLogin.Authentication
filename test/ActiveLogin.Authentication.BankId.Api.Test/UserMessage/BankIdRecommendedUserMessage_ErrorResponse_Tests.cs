using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.UserMessage;

public class BankIdRecommendedUserMessage_ErrorResponse_Tests
{
    private readonly BankIdRecommendedUserMessage _bankIdRecommendedUserMessage;

    public BankIdRecommendedUserMessage_ErrorResponse_Tests()
    {
        _bankIdRecommendedUserMessage = new BankIdRecommendedUserMessage();
    }

    [Theory]
    [InlineData(ErrorCode.Canceled, MessageShortName.RFA3)]
    [InlineData(ErrorCode.AlreadyInProgress, MessageShortName.RFA4)]
    [InlineData(ErrorCode.RequestTimeout, MessageShortName.RFA5)]
    [InlineData(ErrorCode.Maintenance, MessageShortName.RFA5)]
    [InlineData(ErrorCode.InternalError, MessageShortName.RFA5)]
    public void GetMessageShortNameForErrorResponse_ShouldReturnShortMessage_When_Known(ErrorCode errorCode, MessageShortName expected)
    {
        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForErrorResponse(errorCode);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ErrorCode.InvalidParameters)]
    [InlineData(ErrorCode.Unauthorized)]
    [InlineData(ErrorCode.UnsupportedMediaType)]
    [InlineData(ErrorCode.NotFound)]
    [InlineData(ErrorCode.Unknown)]
    public void GetMessageShortNameForErrorResponse_ShouldReturn_RFA22_When_Unknown(ErrorCode errorCode)
    {
        var result = _bankIdRecommendedUserMessage.GetMessageShortNameForErrorResponse(errorCode);

        Assert.Equal(MessageShortName.RFA22, result);
    }
}
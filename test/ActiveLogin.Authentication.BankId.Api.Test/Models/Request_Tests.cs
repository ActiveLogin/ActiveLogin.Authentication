using System;

using ActiveLogin.Authentication.BankId.Api.Models;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models;

public class Request_Tests
{
    [Fact]
    public void Constructor_WhenSignRequest_WhenNoUserVisibleData_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new SignRequest("", null));
        Assert.Equal("userVisibleData", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenPaymentRequest_WhenNoUserVisibleTransaction_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new PaymentRequest("", null));
        Assert.Equal("userVisibleTransaction", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenAnyRequest_WhenNoEndUserIp_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AnyRequest(null));
        Assert.Equal("endUserIp", exception.ParamName);
    }

    class AnyRequest(string endUserIp) : Request(endUserIp, null);
}

using System;

using ActiveLogin.Authentication.BankId.Api.Models;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models;

public class PhoneRequest_Tests
{
    [Fact]
    public void Constructor_WhenPhoneSignRequest_WhenNoUserVisibleData_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new PhoneSignRequest("", CallInitiator.User, null));
        Assert.Equal("userVisibleData", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenAnyRequest_WhenNoPersonalIdentityNumber_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AnyPhoneRequest(null, CallInitiator.User));
        Assert.Equal("personalIdentityNumber", exception.ParamName);
    }

    [Theory]
    [InlineData("1801012392")] // Missing century, only 10 digits
    [InlineData("20180101-2392")] // Uses a dash between date and number
    [InlineData("201801012393")] // Wrong control number, should be 2
    public void Constructor_WhenAnyRequest_WhenInvalidPersonalIdentityNumber_ShouldThrow(string personalIdentityNumber)
    {
        var exception = Assert.Throws<ArgumentException>(() => new AnyPhoneRequest(personalIdentityNumber, CallInitiator.User));
        Assert.Equal("personalIdentityNumber", exception.ParamName);
    }

    class AnyPhoneRequest(string personalIdentityNumber, CallInitiator callInitiator)
        : PhoneRequest(personalIdentityNumber, callInitiator, null);
}

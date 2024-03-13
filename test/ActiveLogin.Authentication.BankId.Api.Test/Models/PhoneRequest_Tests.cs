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

    [Fact]
    public void Constructor_WhenAnyRequest_WhenUnknownCallInitiator_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentException>(() => new AnyPhoneRequest("", CallInitiator.Unknown));
        Assert.Equal("callInitiator", exception.ParamName);
    }

    class AnyPhoneRequest(string personalIdentityNumber, CallInitiator callInitiator)
        : PhoneRequest(personalIdentityNumber, callInitiator, null);
}

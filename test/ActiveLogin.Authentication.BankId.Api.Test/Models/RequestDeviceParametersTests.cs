#nullable enable
using System;
using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models;

public class Request_DeviceParameters_Tests
{
    public class UnHandledDeviceParametersClass() : DeviceParameters("");

    public class DeviceParameterTests : TheoryData<string, DeviceParameterTests.TestData>
    {
        private static readonly AppDeviceParameters FakeAppDeviceParameters = new (
            appIdentifier: "appIdentifier",
            deviceOs: "deviceOs",
            deviceModelName: "deviceModelName",
            deviceIdentifier: "deviceIdentifier");

        private static readonly WebDeviceParameters FakeWebDeviceParameters = new (
            referringDomain: "referringDomain",
            userAgent: "userAgent",
            deviceIdentifier: "deviceIdentifier");

        public DeviceParameterTests()
        {
            Add("No DeviceParameters given", new TestData
            {
                DeviceParameters = null,
                IsTrue = request =>
                    request.AppDeviceParameters == null &&
                    request.WebDeviceParameters == null
            });

            Add("Only AppDeviceParameters should be set", new TestData
            {
                DeviceParameters = FakeAppDeviceParameters,
                IsTrue = request => 
                    request.AppDeviceParameters != null &&
                    request.WebDeviceParameters == null &&
                    request.AppDeviceParameters == FakeAppDeviceParameters // Using reference equality
            });

            Add("Only WebDeviceParameters should be set", new TestData
            {
                DeviceParameters = FakeWebDeviceParameters,
                IsTrue = request =>
                    request.AppDeviceParameters == null &&
                    request.WebDeviceParameters != null &&
                    request.WebDeviceParameters == FakeWebDeviceParameters // Using reference equality
            });

            Add("Should not assign unknown DeviceParameters to property", new TestData
            {
                DeviceParameters = new UnHandledDeviceParametersClass(),
                IsTrue = request =>
                    request.AppDeviceParameters == null &&
                    request.WebDeviceParameters == null
            });
        }

        public class TestData
        {
            public DeviceParameters? DeviceParameters { get; set; }
            public required Func<Request, bool> IsTrue { get; init; }
        }
    }

    [Theory, ClassData(typeof(DeviceParameterTests))]
    public void Constructor_With_DeviceParameters(string description, DeviceParameterTests.TestData data)
    {
        var sut = new TestRequest(data.DeviceParameters);
        Assert.True(data.IsTrue(sut), description);
    }

    public class TestRequest(DeviceParameters? deviceParameters) : Request(
        endUserIp: "",
        requirement: null,
        userVisibleData: null,
        userNonVisibleData: null,
        userVisibleDataFormat: null,
        returnUrl: null,
        returnRisk: null,
        deviceParameters: deviceParameters);

}

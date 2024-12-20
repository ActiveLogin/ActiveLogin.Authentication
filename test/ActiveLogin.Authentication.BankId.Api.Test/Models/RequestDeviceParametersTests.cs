#nullable enable
using System;
using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models;

public class Request_DeviceParameters_Tests
{
    public class DeviceParameterTests : TheoryData<string, DeviceParameterTests.TestData>
    {
        private static readonly DeviceDataApp FakeBankIdEndUserAppDeviceParameters = new (
            appIdentifier: "appIdentifier",
            deviceOs: "deviceOs",
            deviceModelName: "deviceModelName",
            deviceIdentifier: "deviceIdentifier");

        private static readonly DeviceDataWeb FakeBankIdEndUserWebDeviceParameters = new (
            referringDomain: "referringDomain",
            userAgent: "userAgent",
            deviceIdentifier: "deviceIdentifier");

        public DeviceParameterTests()
        {
            Add("No DeviceData given", new TestData
            {
                DeviceParameters = null,
                IsTrue = request =>
                    request.AppDeviceParameters == null &&
                    request.WebDeviceParameters == null
            });

            Add("Only BankIdEndUserAppDeviceParameters should be set", new TestData
            {
                DeviceParameters = FakeBankIdEndUserAppDeviceParameters,
                IsTrue = request => 
                    request.AppDeviceParameters != null &&
                    request.WebDeviceParameters == null &&
                    request.AppDeviceParameters == FakeBankIdEndUserAppDeviceParameters // Using reference equality
            });

            Add("Only BankIdEndUserWebDeviceParameters should be set", new TestData
            {
                DeviceParameters = FakeBankIdEndUserWebDeviceParameters,
                IsTrue = request =>
                    request.AppDeviceParameters == null &&
                    request.WebDeviceParameters != null &&
                    request.WebDeviceParameters == FakeBankIdEndUserWebDeviceParameters // Using reference equality
            });

        }

        public class TestData
        {
            public DeviceData? DeviceParameters { get; set; }
            public required Func<Request, bool> IsTrue { get; init; }
        }
    }

    [Theory, ClassData(typeof(DeviceParameterTests))]
    public void Constructor_With_DeviceParameters(string description, DeviceParameterTests.TestData data)
    {
        var sut = new TestRequest(data.DeviceParameters);
        Assert.True(data.IsTrue(sut), description);
    }

    public class TestRequest(DeviceData? deviceParameters) : Request(
        endUserIp: "",
        requirement: null,
        userVisibleData: null,
        userNonVisibleData: null,
        userVisibleDataFormat: null,
        returnUrl: null,
        returnRisk: null,
        deviceParameters: deviceParameters);

}

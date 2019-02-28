using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserMessage
{
    public class BankIdSupportedDeviceDetector_Detect_Tests
    {
        private readonly BankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;

        public BankIdSupportedDeviceDetector_Detect_Tests()
        {
            _bankIdSupportedDeviceDetector = new BankIdSupportedDeviceDetector();
        }

        [Fact]
        public void Should_Only_Detect_Ios_And_Mobile_When_Iphone_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDekstop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);
        }

        [Fact]
        public void Should_Only_Detect_Ios_And_Mobile_When_Ipad_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (iPad; CPU OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDekstop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);
        }

        [Theory]
        [InlineData("Mozilla/5.0 (Linux; Android 6.0.1; SM-G532G Build/MMB29T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.83 Mobile Safari/537.36")]
        [InlineData("Mozilla/5.0 (Linux; Android 9; BLA-L29) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 Mobile Safari/537.36")]
        public void Should_Only_Detect_Android_And_Mobile_When_Android_User_Agent(string userAgent)
        {
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.True(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDekstop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);
        }


        [Fact]
        public void Should_Only_Detect_WindowsPhone_And_Mobile_When_WindowsPhone_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Mobile Safari/537.36 Edge/16.16299";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.True(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDekstop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);
        }

        [Fact]
        public void Should_Only_Detect_WindowsDesktop_And_Desktop_When_Windows_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.True(detectedDevice.IsWindowsDekstop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.False(detectedDevice.IsMobile);
            Assert.True(detectedDevice.IsDesktop);
        }

        [Fact]
        public void Should_Only_Detect_MacOs_And_Desktop_When_MacOs_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/601.7.7 (KHTML, like Gecko) Version/9.1.2 Safari/601.7.7";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDekstop);
            Assert.True(detectedDevice.IsMacOs);

            Assert.False(detectedDevice.IsMobile);
            Assert.True(detectedDevice.IsDesktop);
        }
    }
}

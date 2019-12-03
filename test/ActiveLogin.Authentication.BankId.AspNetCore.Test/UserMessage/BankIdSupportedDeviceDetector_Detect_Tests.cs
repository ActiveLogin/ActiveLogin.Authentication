using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
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
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.True(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Only_Detect_Ios_And_Mobile_When_Ipad_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (iPad; CPU OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.True(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
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
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.True(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }


        [Fact]
        public void Should_Only_Detect_WindowsPhone_And_Mobile_When_WindowsPhone_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Mobile Safari/537.36 Edge/16.16299";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.True(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.True(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Only_Detect_WindowsDesktop_And_Desktop_When_Windows_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.True(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.False(detectedDevice.IsMobile);
            Assert.True(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.True(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Only_Detect_MacOs_And_Desktop_When_MacOs_User_Agent()
        {
            var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/601.7.7 (KHTML, like Gecko) Version/9.1.2 Safari/601.7.7";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.True(detectedDevice.IsMacOs);

            Assert.False(detectedDevice.IsMobile);
            Assert.True(detectedDevice.IsDesktop);

            Assert.True(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Safari_On_IOS()
        {
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.True(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Chrome_On_IOS()
        {
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.True(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Firefox_On_IOS()
        {
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_3 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) FxiOS/1.0 Mobile/12F69 Safari/600.1.4";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.True(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Edge_On_IOS()
        {
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89 Safari/603.2.4 EdgiOS/41.1.35.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.True(detectedDevice.IsIos);
            Assert.False(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.True(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Chrome_On_Android()
        {
            var userAgent = "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.True(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.True(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Firefox_On_Android()
        {
            var userAgent = "Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.True(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.True(detectedDevice.IsFirefox);
            Assert.False(detectedDevice.IsEdge);
        }

        [Fact]
        public void Should_Detect_Edge_On_Android()
        {
            var userAgent = "Mozilla/5.0 (Linux; Android 8.0; Pixel XL Build/OPP3.170518.006) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.0 Mobile Safari/537.36 EdgA/41.1.35.1";
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(userAgent);

            Assert.False(detectedDevice.IsIos);
            Assert.True(detectedDevice.IsAndroid);
            Assert.False(detectedDevice.IsWindowsPhone);
            Assert.False(detectedDevice.IsWindowsDesktop);
            Assert.False(detectedDevice.IsMacOs);

            Assert.True(detectedDevice.IsMobile);
            Assert.False(detectedDevice.IsDesktop);

            Assert.False(detectedDevice.IsSafari);
            Assert.False(detectedDevice.IsChrome);
            Assert.False(detectedDevice.IsFirefox);
            Assert.True(detectedDevice.IsEdge);
        }
    }
}

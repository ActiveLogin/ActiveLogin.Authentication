using System;

using ActiveLogin.Authentication.BankId.Core.Launcher;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;

/// <summary>
/// Comprehensive tests for BankIdRedirectUrl.TryCreate covering:
/// - URL validation (null, empty, whitespace)
/// - URL encoding requirements
/// - Maximum length validation (512 characters after encoding)
/// - Browser-specific scheme replacement (iOS Chrome and Firefox)
/// - Custom browser configuration override
/// - URL fragment preservation (for nonce binding)
/// - Different browser/OS combinations
///
/// Based on BankID Return URL specification:
/// https://developers.bankid.com/getting-started/return-url
/// </summary>
public class BankIdRedirectUriTests
{
    #region Validation Tests

    [Fact]
    public void TryCreate_ShouldFail_WhenUrlIsNull()
    {
        // Act
        var result = BankIdRedirectUrl.TryCreate(
            null!,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect null validation

        // Assert
        result.Match(
            _ => Assert.Fail("Expected failure but got success"),
            error => Assert.Equal("Invalid URL", error)
        );
    }

    [Fact]
    public void TryCreate_ShouldFail_WhenUrlIsEmpty()
    {
        // Act
        var result = BankIdRedirectUrl.TryCreate(
            string.Empty,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect empty string validation

        // Assert
        result.Match(
            _ => Assert.Fail("Expected failure but got success"),
            error => Assert.Equal("Invalid URL", error)
        );
    }

    [Fact]
    public void TryCreate_ShouldFail_WhenUrlIsWhitespace()
    {
        // Act
        var result = BankIdRedirectUrl.TryCreate(
            "   ",
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect whitespace validation

        // Assert
        Assert.True(result.IsFailure);
        result.Match(
            _ => Assert.Fail("Expected failure but got success"),
            error => Assert.Equal("Invalid URL", error)
        );
    }

    [Fact]
    public void TryCreate_ShouldFail_WhenEncodedUrlExceeds512Characters()
    {
        // Arrange - Create URL that when encoded exceeds 512 chars
        var longUrl = "https://example.com/" + new string('a', 500);

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            longUrl,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect length validation

        // Assert
        Assert.True(result.IsFailure);
        result.Match(
            _ => Assert.Fail("Expected failure but got success"),
            error => Assert.Equal("URL must be at most 512 characters long", error)
        );
    }

    [Fact]
    public void TryCreate_ShouldSucceed_WhenEncodedUrlIsExactly512Characters()
    {
        // Arrange - Calculate URL that encodes to exactly 512 characters
        // "https://example.com/" encodes to "https%3A%2F%2Fexample.com%2F" (28 chars)
        // Need 512 - 28 = 484 more encoded characters
        // 'a' encodes to 'a' (1 char), so we need 484 'a's
        var url = "https://example.com/" + new string('a', 484);

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect length validation

        // Assert
        Assert.True(result.IsSuccess, "URL with exactly 512 encoded characters should succeed");
        result.Match(
            redirectUrl => Assert.Equal(512, redirectUrl.Url.Length),
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion

    #region URL Encoding Tests

    [Fact]
    public void TryCreate_ShouldEncodeUrl_UsingUriEscapeDataString()
    {
        // Arrange
        var url = "https://example.com/path?param=value&other=test";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect URL encoding

        // Assert
        result.Match(
            redirectUrl => {
                Assert.Equal(Uri.EscapeDataString(url), redirectUrl.Url);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldPreserveUrlFragment_ForNonceBinding()
    {
        // Arrange - Fragment with nonce as per BankID spec
        var url = "https://example.com/login#nonce=session123";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect fragment preservation

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.Contains("#nonce=session123", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldEncodeSpecialCharacters()
    {
        // Arrange
        var url = "https://example.com/path?special=åäö&chars=spaces here";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect character encoding

        // Assert
        result.Match(
            redirectUrl => {
                Assert.DoesNotContain("å", redirectUrl.Url);
                Assert.DoesNotContain("ä", redirectUrl.Url);
                Assert.DoesNotContain("ö", redirectUrl.Url);
                Assert.DoesNotContain(" ", redirectUrl.Url);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion

    #region iOS Browser Scheme Replacement Tests

    [Fact]
    public void TryCreate_ShouldReplaceHttpsWithChromeScheme_ForIosChrome()
    {
        // Arrange
        var url = "https://example.com/return?";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.Mobile.Ios.Chrome); // Testing iOS Chrome-specific behavior

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("chromebrowser://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldReplaceHttpsWithFirefoxScheme_ForIosFirefox()
    {
        // Arrange
        var url = "https://example.com/return";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.Mobile.Ios.Firefox); // Testing iOS Firefox-specific behavior

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("firefox://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldNotReplaceScheme_ForIosSafari()
    {
        // Arrange
        var url = "https://example.com/return";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.Mobile.Ios.Safari); // Testing iOS Safari-specific behavior

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("https://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldNotReplaceScheme_ForIosEdge()
    {
        // Arrange
        var url = "https://example.com/return";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.Mobile.Ios.Edge); // Testing iOS Edge-specific behavior

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("https://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion

    #region Android Browser Tests

    [Theory]
    [InlineData(nameof(BankIdTestDevices.Mobile.Android.Chrome))]
    [InlineData(nameof(BankIdTestDevices.Mobile.Android.Firefox))]
    [InlineData(nameof(BankIdTestDevices.Mobile.Android.Edge))]
    [InlineData(nameof(BankIdTestDevices.Mobile.Android.SamsungInternet))]
    [InlineData(nameof(BankIdTestDevices.Mobile.Android.Opera))]
    public void TryCreate_ShouldNotReplaceScheme_ForAndroidBrowsers(string browserName)
    {
        // Arrange
        var url = "https://example.com/return";
        var device = browserName switch // Testing Android browser-specific behavior
        {
            nameof(BankIdTestDevices.Mobile.Android.Chrome) => BankIdTestDevices.Mobile.Android.Chrome,
            nameof(BankIdTestDevices.Mobile.Android.Firefox) => BankIdTestDevices.Mobile.Android.Firefox,
            nameof(BankIdTestDevices.Mobile.Android.Edge) => BankIdTestDevices.Mobile.Android.Edge,
            nameof(BankIdTestDevices.Mobile.Android.SamsungInternet) => BankIdTestDevices.Mobile.Android.SamsungInternet,
            nameof(BankIdTestDevices.Mobile.Android.Opera) => BankIdTestDevices.Mobile.Android.Opera,
            _ => throw new ArgumentException($"Unknown browser: {browserName}")
        };

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device);

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("https://", decodedUrl);
            },
            _ => Assert.Fail($"Expected success for {browserName} but got failure")
        );
    }

    #endregion

    #region Desktop Browser Tests

    [Theory]
    [InlineData(nameof(BankIdTestDevices.Desktop.Windows_Chrome))]
    [InlineData(nameof(BankIdTestDevices.Desktop.Windows_Edge))]
    [InlineData(nameof(BankIdTestDevices.Desktop.Windows_Firefox))]
    [InlineData(nameof(BankIdTestDevices.Desktop.MacOs_Safari))]
    [InlineData(nameof(BankIdTestDevices.Desktop.MacOs_Chrome))]
    public void TryCreate_ShouldNotReplaceScheme_ForDesktopBrowsers(string browserName)
    {
        // Arrange
        var url = "https://example.com/return";
        var device = browserName switch // Testing Desktop browser-specific behavior
        {
            nameof(BankIdTestDevices.Desktop.Windows_Chrome) => BankIdTestDevices.Desktop.Windows_Chrome,
            nameof(BankIdTestDevices.Desktop.Windows_Edge) => BankIdTestDevices.Desktop.Windows_Edge,
            nameof(BankIdTestDevices.Desktop.Windows_Firefox) => BankIdTestDevices.Desktop.Windows_Firefox,
            nameof(BankIdTestDevices.Desktop.MacOs_Safari) => BankIdTestDevices.Desktop.MacOs_Safari,
            nameof(BankIdTestDevices.Desktop.MacOs_Chrome) => BankIdTestDevices.Desktop.MacOs_Chrome,
            _ => throw new ArgumentException($"Unknown browser: {browserName}")
        };

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device);

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("https://", decodedUrl);
            },
            _ => Assert.Fail($"Expected success for {browserName} but got failure")
        );
    }

    #endregion

    #region Custom Browser Configuration Tests

    [Fact]
    public void TryCreate_ShouldUseCustomBrowserScheme_WhenConfigProvided()
    {
        // Arrange
        var url = "https://example.com/return";
        var customScheme = new BrowserScheme("myapp://");
        var config = new BankIdLauncherCustomBrowserConfig(
            customScheme,
            BrowserMightRequireUserInteractionToLaunch.Never
        );

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config, // Testing custom config override
            device: BankIdTestDevices.AnyDevice); // Device irrelevant when custom config provided

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("myapp://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldPreferCustomScheme_OverDefaultIosChromeScheme()
    {
        // Arrange
        var url = "https://example.com/return";
        var customScheme = new BrowserScheme("customapp://");
        var config = new BankIdLauncherCustomBrowserConfig(
            customScheme,
            BrowserMightRequireUserInteractionToLaunch.Never
        );

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config, // Testing that custom config takes precedence
            device: BankIdTestDevices.Mobile.Ios.Chrome); // Would normally use chromebrowsers://

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.StartsWith("customapp://", decodedUrl);
                Assert.DoesNotContain("chromebrowser://", decodedUrl);
                Assert.DoesNotContain("https://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldHandleCustomSchemeWithTrailingSlashes()
    {
        // Arrange
        var url = "https://example.com/return";
        var customScheme = new BrowserScheme("myapp://");
        var config = new BankIdLauncherCustomBrowserConfig(
            customScheme,
            BrowserMightRequireUserInteractionToLaunch.Never
        );

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config, // Testing BrowserScheme trimming behavior
            device: BankIdTestDevices.AnyDevice); // Device irrelevant when custom config provided

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                // BrowserScheme trims trailing :// so we expect clean scheme
                Assert.StartsWith("myapp://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion

    #region Case Sensitivity Tests

    [Fact]
    public void TryCreate_ShouldReplaceHttps_CaseInsensitive()
    {
        // Arrange - mixed case https
        var url = "HTTPS://www.example.com/return";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.Mobile.Ios.Chrome); // Testing case-insensitive replacement for iOS Chrome

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.Equal("chromebrowser://www.example.com/return", decodedUrl);
                Assert.DoesNotContain("HTTPS://", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion

    #region Implicit Conversion Tests

    [Fact]
    public void ImplicitConversion_ShouldReturnUrl_WhenBankIdRedirectUrlIsNotNull()
    {
        // Arrange
        var url = "https://example.com/return";
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect implicit conversion

        // Act
        string convertedUrl = null;
        result.Match(
            redirectUrl => convertedUrl = redirectUrl, // Implicit conversion to string
            _ => Assert.Fail("Expected success")
        );

        // Assert
        Assert.NotNull(convertedUrl);
        Assert.Equal(Uri.EscapeDataString(url), convertedUrl);
    }

    [Fact]
    public void ToString_ShouldReturnUrl()
    {
        // Arrange
        var url = "https://example.com/return";
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect ToString

        // Act & Assert
        result.Match(
            redirectUrl => {
                Assert.Equal(redirectUrl.Url, redirectUrl.ToString());
            },
            _ => Assert.Fail("Expected success")
        );
    }

    #endregion

    #region Real-World Scenario Tests

    [Fact]
    public void TryCreate_ShouldHandleTypicalLoginUrl_WithNonce()
    {
        // Arrange - Typical scenario from BankID docs
        var url = "https://www.example.com/login#nonce=abc123def456";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't matter for URL structure test

        // Assert
        result.Match(
            redirectUrl => {
                Assert.NotNull(redirectUrl.Url);
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.Contains("login", decodedUrl);
                Assert.Contains("nonce=abc123def456", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    [Fact]
    public void TryCreate_ShouldHandleLocalhost_WithPort()
    {
        // Arrange - Development scenario
        var url = "https://localhost:5001/ActiveLogin/BankId/Auth/Init?returnUrl=/secure";

        // Act
        var result = BankIdRedirectUrl.TryCreate(
            url,
            config: null,
            device: BankIdTestDevices.AnyDevice); // Device doesn't affect localhost handling

        // Assert
        result.Match(
            redirectUrl => {
                var decodedUrl = Uri.UnescapeDataString(redirectUrl.Url);
                Assert.Contains("localhost:5001", decodedUrl);
            },
            _ => Assert.Fail("Expected success but got failure")
        );
    }

    #endregion
}

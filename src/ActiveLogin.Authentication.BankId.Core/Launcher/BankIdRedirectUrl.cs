using System.Diagnostics.CodeAnalysis;

using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;


/*
Android with Edge: bankid://
*/

/// <summary>
/// The redirect URL: <br/>
/// * Must be UTF-8 and URL encoded. <br/>
/// * Should take the user back to the same webpage. <br/>
/// * May include parameters to be passed to the browser. <br/>
/// * Can be set to null.
/// </summary>
public record BankIdRedirectUrl
{
    private const string IosChromeScheme = "chromebrowser";
    private const string IosFirefoxScheme = "firefox";

    public required string Url { get; init; }

    private BankIdRedirectUrl(){}

    public static Result<BankIdRedirectUrl> TryCreate(
        string redirectUrl,
        BankIdLauncherCustomBrowserConfig? config,
        BankIdSupportedDevice device
    )
    {
        if (string.IsNullOrWhiteSpace(redirectUrl))
        {
            return "Invalid URL";
        }

        // only https allowed
        if (!redirectUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return "Only https scheme is allowed in redirect URL";
        }

        // get custom browser scheme if any, otherwise get scheme based on device
        var browserScheme = config?.BrowserScheme is not null
            ? config.BrowserScheme
            : GetScheme(device);

        redirectUrl = !string.IsNullOrWhiteSpace(browserScheme)
            ? redirectUrl.Replace("https", browserScheme, StringComparison.OrdinalIgnoreCase)
            : redirectUrl;

        var url = Uri.EscapeDataString(redirectUrl);

        if (512 < url.Length)
        {
            return "URL must be at most 512 characters long";
        }

        return new BankIdRedirectUrl
        {
            Url = url
        };
    }

    private static string? GetScheme(BankIdSupportedDevice device)
    {
        // This will launch the browser with the last page used (the Active Login status page).
        // If a URL is specified these browsers will open that URL in a new tab and we will lose context.
        return device.DeviceOs switch
        {
            BankIdSupportedDeviceOs.Ios => device.DeviceBrowser switch
            {
                BankIdSupportedDeviceBrowser.Chrome => IosChromeScheme, // Normally you would supply the URL, but we just want to launch the app again
                BankIdSupportedDeviceBrowser.Firefox => IosFirefoxScheme, // Normally you would supply the URL, but we just want to launch the app again
                _ => null
            },
            _ => null
        };
    }

    public override string ToString() => Url;

    [return: NotNullIfNotNull(nameof(bankIdRedirectUrl))]
    public static implicit operator string?(BankIdRedirectUrl? bankIdRedirectUrl) => bankIdRedirectUrl?.Url;
};

/*
# Return URL
When the BankID client is used on the same device as the user visits your service on,
the user should be sent back to your service once they've completed their action in the BankID client.
From version 6 of our RP-API, it's possible and strongly recommended to send the return URL via server,
meaning that you can protect the user and ensure that any sessions started from your service will send them back to your service.
This can mitigate some session fixation cases.

Note that usage of return URL as part of autostart URL is deprecated. Instead, the return URL
should be included in the RP-API call when starting an order.

## How it works

### For Web
1. The user visits your website at https://www.example.com
2. The user chooses to log in. https://www.example.com/login
3. Your website makes a call to our RP-API which includes, among other things, the return URL: https://www.example.com/login#nonce=[session nonce]
4. The RP-API returns the autostart token.
5. Your website starts the BankID client using the autostart URL. Example: https://app.bankid.com/?autostarttoken=[autostarttoken from step 4]
6. The user completes the identification in the BankID app.
7. The BankID app invokes the return URL, previously sent to the RP-API in step 3.
8. Your website verifies that the nonce matches the session nonce.

### For App
1. The user starts your app.
2. The user chooses to log in.
3. Your app makes a server side call to our RP-API, which includes, among other things, the app specific return URL: https://app.example.com/login#nonce=[session nonce] or myapp://login#nonce=[session nonce].
4. The RP-API returns the autostart token.
5. Your app starts the BankID client using the autostart URL. Example: https://app.bankid.com/?autostarttoken=[autostarttoken from step 4]
6. The user completes the identification in the BankID app.
7. The BankID app invokes the return URL, previously sent to the RP-API in step 3.
8. Your app verifies that the nonce matches the session nonce.

Please note: If the user has an older desktop version of the BankID client that doesn't support getting the
return URL from the server, the order will be cancelled and the user will be asked to update their app.

## Pitfalls in web flows
In cases where the user visits your service in a web browser you can face some challenges when it comes to returning
them to the right broswser or app. This is due to device settings and/or differences in browser behaviour and is
out of our control. However, there are some things you can do to enhance the probability that the user is
returned to the right browser and/or tab.

## Returning the user to correct browser
The return URL will be opened in the default browser, even if the user started the session in a different browser.

If the used brower has a documented URL scheme this should be used in the return URL instead of 'https'.

Example: https://www.example.com/ for Google Chrome would be: chromebrowser://www.example.com/.

Please note: For browser without a documented URL scheme, the user will be sent to the default browser.

## Returning the user to correct tab
For a good user experience, returning the user to the same tab is preferred.
However, it can be a challenge to do this from an app, such as BankID security app.

The highest probability to return the user to the tab where they started BankID from,
is to use the exact URL of the target tab as the return URL.
However, to bind the new request to the old browser session,
the return URL should include a nonce that server side will match to the old browser session.
The best way to accomplish this is to place the nonce in the URL fragment.
Please note that the nonce should not contain sensitive session information such as the session cookie.

Example: https://www.example.com/login#nonce=[session nonce]

## Returning the user to correct browser and tab
When combining the methods above, some browsers may deliver unexpected results. This is because different browser behave differently when given the same input. It's important that you test different scenarios for your service.

*/

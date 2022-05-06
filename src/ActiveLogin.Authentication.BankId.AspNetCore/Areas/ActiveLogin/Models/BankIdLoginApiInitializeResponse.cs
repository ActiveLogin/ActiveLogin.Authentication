namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdLoginApiInitializeResponse
{
    internal BankIdLoginApiInitializeResponse(
        bool isAutoLaunch,
        bool deviceMightRequireUserInteractionToLaunchBankIdApp,
        bool checkStatus,
        string orderRef,
        string? redirectUri,
        string? qrStartState,
        string? qrCodeAsBase64)
    {
        IsAutoLaunch = isAutoLaunch;
        DeviceMightRequireUserInteractionToLaunchBankIdApp = deviceMightRequireUserInteractionToLaunchBankIdApp;
        CheckStatus = checkStatus;
        OrderRef = orderRef;
        RedirectUri = redirectUri;
        QrStartState = qrStartState;
        QrCodeAsBase64 = qrCodeAsBase64;
    }


    public bool IsAutoLaunch { get; }
    public bool DeviceMightRequireUserInteractionToLaunchBankIdApp { get; }
    public bool CheckStatus { get; }
    public string OrderRef { get; }
    public string? RedirectUri { get; }
    public string? QrStartState { get; set; }
    public string? QrCodeAsBase64 { get; set; }


    public static BankIdLoginApiInitializeResponse AutoLaunch(string orderRef, string redirectUri, bool showLaunchButton)
    {
        return new BankIdLoginApiInitializeResponse(true, showLaunchButton, false, orderRef, redirectUri, null, null);
    }

    public static BankIdLoginApiInitializeResponse AutoLaunchAndCheckStatus(string orderRef, string redirectUri, bool showLaunchButton)
    {
        return new BankIdLoginApiInitializeResponse(true, showLaunchButton, true, orderRef, redirectUri, null, null);
    }

    public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef, string qrStartState, string qrCodeAsBase64)
    {
        return new BankIdLoginApiInitializeResponse(false, false, true, orderRef, null, qrStartState, qrCodeAsBase64);
    }

    public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef)
    {
        return new BankIdLoginApiInitializeResponse(false, false, true, orderRef, null, null, null);
    }
}
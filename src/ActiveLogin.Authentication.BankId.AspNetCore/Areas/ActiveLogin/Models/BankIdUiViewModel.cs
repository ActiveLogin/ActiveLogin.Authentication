namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiViewModel
{
    internal BankIdUiViewModel(BankIdUiScriptConfiguration bankUiScriptConfiguration, BankIdUiScriptInitState bankIdUiScriptInitState, BankIdUiSignData? signData = null, BankIdUiPaymentData? paymentData = null)
    {
        BankUiScriptConfiguration = bankUiScriptConfiguration;
        BankIdUiScriptInitState = bankIdUiScriptInitState;

        BankIdUiSignData = signData;
        BankIdUiPaymentData = paymentData;
    }

    public string? LocalizedPageHeader { get; init; }
    public string? LocalizedPageTitle { get; init; }
    public string? LocalizedStartAppButtonText { get; init; }
    public string? LocalizedCancelButtonText { get; init; }
    public string? LocalizedQrCodeImageAltText { get; init; }

    public BankIdUiScriptConfiguration BankUiScriptConfiguration { get; }

    public BankIdUiScriptInitState BankIdUiScriptInitState { get; }

    public BankIdUiSignData? BankIdUiSignData { get; }
    public BankIdUiPaymentData? BankIdUiPaymentData { get; }
}

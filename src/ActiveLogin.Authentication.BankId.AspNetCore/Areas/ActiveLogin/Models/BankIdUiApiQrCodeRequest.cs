using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiApiQrCodeRequest
{
    public BankIdUiApiQrCodeRequest()
    {

    }

    [Required]
    public string? QrStartState { get; set; }
}

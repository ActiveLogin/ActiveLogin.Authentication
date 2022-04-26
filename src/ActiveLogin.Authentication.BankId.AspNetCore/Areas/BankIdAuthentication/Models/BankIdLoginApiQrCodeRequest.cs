using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;

public class BankIdLoginApiQrCodeRequest
{
    public BankIdLoginApiQrCodeRequest()
    {

    }

    [Required]
    public string? QrStartState { get; set; }
}

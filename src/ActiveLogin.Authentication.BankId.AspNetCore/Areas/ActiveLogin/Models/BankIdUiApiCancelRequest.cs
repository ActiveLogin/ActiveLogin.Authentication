using System.ComponentModel.DataAnnotations;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiApiCancelRequest
{
    public BankIdUiApiCancelRequest()
    {

    }

    [Required]
    public string? UiOptions { get; set; }

    [Required]
    public string? OrderRef { get; set; }
}

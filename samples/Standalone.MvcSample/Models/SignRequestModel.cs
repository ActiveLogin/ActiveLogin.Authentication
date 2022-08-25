namespace Standalone.MvcSample.Models;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class SignRequestModel
{
    public string ReturnUrl { get; set; }

    public IFormFile FileToSign { get; set; }
}

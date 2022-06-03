using System.Text;
using System.Xml.Linq;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Standalone.MvcSample.Models;

namespace Standalone.MvcSample.Controllers;

//
// DISCLAIMER
//
// These are samples on how to use Active Login in different situations
// and might not represent optimal way of setting up
// ASP.NET MVC or other components.
//
// Please see this as inspiration, not a complete template.
//

[AllowAnonymous]
public class SignController : Controller
{
    private readonly IBankIdSignConfigurationProvider _bankIdSignConfigurationProvider;
    private readonly IBankIdSignService _bankIdSignService;

    public SignController(IBankIdSignConfigurationProvider bankIdSignConfigurationProvider, IBankIdSignService bankIdSignService)
    {
        _bankIdSignConfigurationProvider = bankIdSignConfigurationProvider;
        _bankIdSignService = bankIdSignService;
    }

    public async Task<IActionResult> Index()
    {
        var configurations = await _bankIdSignConfigurationProvider.GetAllConfigurationsAsync();
        var providers = configurations
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Key, x.Key));
        var viewModel = new BankIdViewModel(providers, $"{Url.Action(nameof(Index))}");

        return View(viewModel);
    }

    [AllowAnonymous]
    [HttpPost("Sign")]
    public async Task<IActionResult> Sign([FromQuery]string provider, [FromForm] SignRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var userVisibleContent = await GetUserVisibleData(model.FileToSign);
        var userNonVisibleData = BitConverter.GetBytes(model.FileToSign.GetHashCode());

        var props = new BankIdSignProperties(userVisibleContent)
        {
            Items =
            {
                {"scheme", provider},
                {"fileName", model.FileToSign.FileName},
                {"fileType", model.FileToSign.ContentType}
            },
            UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1,
            UserNonVisibleData = userNonVisibleData
        };
        var returnPath = $"{Url.Action(nameof(Callback))}?provider={provider}";
        return this.BankIdInitiateSign(props, returnPath, provider);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Callback(string provider)
    {
        var result = await _bankIdSignService.GetSignResultAsync(provider);
        if (result?.Succeeded != true || result.BankIdCompletionData == null)
        {
            throw new Exception("Sign error");
        }
        
        return View("Result", new SignResultViewModel(
            BitConverter.ToInt32(result.Properties.UserNonVisibleData ?? Array.Empty<byte>(), 0),
            result.Properties.Items["fileName"] ?? string.Empty,
            result.Properties.Items["fileType"] ?? string.Empty,
            result.BankIdCompletionData.User.PersonalIdentityNumber,
            result.BankIdCompletionData.User.Name,
            result.BankIdCompletionData.Device.IpAddress,
            FormatXml(Encoding.UTF8.GetString(Convert.FromBase64String(result.BankIdCompletionData.Signature)))
            )
        );
    }

    private static string FormatXml(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }

    private static async Task<string> GetUserVisibleData(IFormFile file)
    {
        var tempFile = Path.Join(Path.GetTempPath(), file.FileName);
        var fileInfo = new FileInfo(tempFile);
        await file.CopyToAsync(fileInfo.OpenWrite());

        var fileSize = FormatFileSize(fileInfo);

        var userVisibleContent = new StringBuilder()
            .AppendLine("# Overview")
            .AppendLine("You are about to sign a file, please verify the information below that you are signing the correct file.")
            .AppendLine("# File information")
            .AppendLine("|||")
            .AppendLine("|-|-|")
            .AppendLine($"|*Name*| {file.FileName} |")
            .AppendLine($"|*Size*| {fileSize} |")
            .AppendLine($"|*Created*| {fileInfo.CreationTime.ToShortDateString()} {fileInfo.CreationTime.ToShortTimeString()} |")
            .AppendLine($"|*Modified*| {fileInfo.LastWriteTime.ToShortDateString()} {fileInfo.LastWriteTime.ToShortTimeString()} |")
            .ToString();

        return userVisibleContent;
    }

    private static string FormatFileSize(FileInfo fileInfo)
    {
        return fileInfo.Length switch
        {
            (>= 1024 and < 1024 * 1024) => $"{fileInfo.Length / 1024.0 : 0.##}kB",
            (>= 1024 * 1024) => $"{fileInfo.Length / 1024 / 1024.0: 0.##}MB",
            _ => $"{fileInfo.Length}bytes",
        };
    }
}

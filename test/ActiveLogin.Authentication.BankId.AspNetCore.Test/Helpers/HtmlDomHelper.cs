using AngleSharp;
using AngleSharp.Dom;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    public static class HtmlDocumentHelper
    {
        public static async Task<IDocument> FromContent(string content)
        {
            var context = BrowsingContext.New(Configuration.Default);
            return await context.OpenAsync(req => req.Content(content));
        }
    }
}

using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    public static class AngleSharpExtensions
    {
        public static T GetElement<T>(this IDocument document, string selector)
        {
            return (T)document.QuerySelector(selector);
        }

        public static string GetInputValue(this IDocument document, string selector)
        {
            return document.QuerySelector(selector) is IHtmlInputElement input ? input.Value : "";
        }

        public static string GetRequestVerificationToken(this IDocument document)
        {
            return document.GetInputValue("input[name='RequestVerificationToken']");
        }
    }
}

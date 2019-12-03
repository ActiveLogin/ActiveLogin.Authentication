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
            var input = document.QuerySelector(selector) as IHtmlInputElement;
            return input != null ? input.Value : "";
        }

        public static string GetRequestVerificationToken(this IDocument document)
        {
            return document.GetInputValue("input[name='RequestVerificationToken']");
        }
        
    }
}

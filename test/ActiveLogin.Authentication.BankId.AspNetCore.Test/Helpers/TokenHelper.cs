using System;
using System.Text.RegularExpressions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    public static class TokenHelper
    {
        public static string ExtractToken(string responseText)
        {
            var match = Regex.Match(responseText, @"\<input name=""RequestVerificationToken"" type=""hidden"" value=""([^""]+)""\>");
            return match.Success ? match.Groups[1].Captures[0].Value : String.Empty;
        }
    }
}

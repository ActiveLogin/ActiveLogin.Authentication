using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
internal static class Validators
{
    public static void ThrowIfNullOrWhitespace([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException(paramName);
        }
    }
}

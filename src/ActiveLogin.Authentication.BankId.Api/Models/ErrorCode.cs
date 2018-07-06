namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public enum ErrorCode
    {
        Unknown,

        // 400
        AlreadyInProgress,
        InvalidParameters,

        // 401
        Unauthorized,

        // 404
        NotFound,

        // 408,
        RequestTimeout,

        // 415,
        UnsupportedMediaType,

        // 500,
        InternalError,

        // 503
        Maintenance
    }
}
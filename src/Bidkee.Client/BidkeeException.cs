using System;
using System.Net;

namespace Bidkee.Client
{
    /// <summary>Protocol or HTTP failure from BidkeePlat.</summary>
    public sealed class BidkeeException : Exception
    {
        public BidkeeException(string code, string message, int statusCode = 0, string? detail = null, string? hint = null)
            : base(message)
        {
            Code = code ?? "ERROR";
            StatusCode = statusCode;
            Detail = detail;
            Hint = hint;
        }

        public string Code { get; }
        public int StatusCode { get; }
        public string? Detail { get; }
        public string? Hint { get; }
        public TimeSpan? RetryAfter { get; set; }

        public bool IsUnauthorized => StatusCode == 401 || string.Equals(Code, "UNAUTHORIZED", StringComparison.OrdinalIgnoreCase);
        public bool IsRateLimited => StatusCode == 429 || string.Equals(Code, "RATE_LIMITED", StringComparison.OrdinalIgnoreCase);

        public static BidkeeException FromStatus(HttpStatusCode status, string? code, string? detail, string? hint = null)
        {
            var n = (int)status;
            var resolved = string.IsNullOrWhiteSpace(code)
                ? (n == 401 ? "UNAUTHORIZED" : n == 429 ? "RATE_LIMITED" : n == 404 ? "NOT_FOUND" : n == 400 ? "MALFORMED" : "HTTP_" + n)
                : code!;
            var message = string.IsNullOrWhiteSpace(detail) ? resolved : resolved + ": " + detail;
            return new BidkeeException(resolved, message, n, detail, hint);
        }
    }
}

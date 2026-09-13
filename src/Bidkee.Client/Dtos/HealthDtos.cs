using System.Text.Json;

namespace Bidkee.Client.Dtos
{
    public sealed class HealthResponse
    {
        public bool Ok { get; set; }
        public string? Protocol { get; set; }
        public string? Signing { get; set; }
        public string? Api { get; set; }
        public bool HsmEnabled { get; set; }
        public string[]? Capabilities { get; set; }
        public JsonElement? Hsm { get; set; }
        public JsonElement? Db { get; set; }
        public JsonElement? Ip { get; set; }
    }

    public sealed class CapabilitiesResponse
    {
        public bool Ok { get; set; }
        public string[]? Capabilities { get; set; }
        public string? PresentUrl { get; set; }
        public string? EnrollUrl { get; set; }
    }

    public sealed class ErrorBody
    {
        public string? Error { get; set; }
        public string? Code { get; set; }
        public string? Detail { get; set; }
        public string? Hint { get; set; }
        public string? Message { get; set; }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bidkee.Client
{
    public static class BidkeeJson
    {
        public static readonly JsonSerializerOptions Options = Create();

        static JsonSerializerOptions Create()
        {
            var o = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };
            o.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            return o;
        }
    }
}

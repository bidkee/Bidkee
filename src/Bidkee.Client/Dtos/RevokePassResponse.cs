namespace Bidkee.Client.Dtos
{
    public sealed class RevokePassResponse
    {
        public bool Ok { get; set; }
        public string? Code { get; set; }
        public string? PassNo { get; set; }
    }
}

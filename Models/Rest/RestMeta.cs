namespace ASP_PV411.Models.Rest
{
    public class RestMeta
    {
        public string Service { get; set; } = null!;
        public string Resouce { get; set; } = null!;
        public string Method { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string DataType { get; set; } = null!;
        public long ServerTime { get; set; }
        public long Cache { get; set; }
        public Dictionary<string, string> Links { get; set; } = [];
    }
}

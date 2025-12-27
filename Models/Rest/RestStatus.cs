namespace ASP_PV411.Models.Rest
{
    public class RestStatus
    {
        public bool IsOk { get; set; }
        public int Code { get; set; }
        public string Phrase { get; set; } = null!;
        public static readonly RestStatus Ok = new() { IsOk = true, Code = 200, Phrase = "OK"};
        public static readonly RestStatus NotFound = new() { IsOk = false, Code = 404, Phrase = "Not Found"};
    }
}

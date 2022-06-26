namespace webapi.Model
{
    public class ResponseInfo
    {
        public int Pages { get; set; }

        public int Count { get; set; }

        public string Prev { get; set; }

        public string Next { get; set; }
    }
}
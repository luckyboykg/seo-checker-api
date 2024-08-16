namespace Domain.Constant
{
    public static class CommonConstant
    {
        public const int TotalResults = 100;

        public const string Google = "Google";
        public const string Bing = "Bing";

        public const string GoogleUrl = "https://www.google.com.au";
        public const string BingUrl = "https://www.bing.com";

        public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

        public const string GoogleUrlPattern = $@"data-id=""atritem-https:\/\/(.*?)\""";
        public const string BingUrlPattern = $@"<cite>https:\/\/(.*?)\""";
    }
}

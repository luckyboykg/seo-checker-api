using Domain.Constant;
using System.Text.RegularExpressions;

namespace Infrastructure.Google
{
    public interface IGoogleSearchService
    {
        Task<string> SearchAsync(string url, string searchPhrase);
    }

    public class GoogleSearchService : IGoogleSearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleSearchService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> SearchAsync(string url, string searchPhrase)
        {
            var htmlContent = await SearchGoogleAsync(searchPhrase);

            var positions = GetPositions(htmlContent, url);

            return positions.Count > 0 ? string.Join(", ", positions) : "0";
        }

        private async Task<string> SearchGoogleAsync(string searchPhrase)
        {
            var url = $"{CommonConstant.GoogleUrl}/search?q={Uri.EscapeDataString(searchPhrase)}&num={CommonConstant.TotalResults}";

            var httpClient = _httpClientFactory.CreateClient("GoogleSearch");
            httpClient.DefaultRequestHeaders.Add("User-Agent", CommonConstant.DefaultUserAgent);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private List<int> GetPositions(string html, string targetUrl)
        {
            var positions = new List<int>();
            var regex = new Regex(CommonConstant.GoogleUrlPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(html).Take(CommonConstant.TotalResults);
            int index = 1;

            foreach (Match match in matches)
            {
                if (match.ToString().Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(index);
                }

                index++;
            }

            return positions;
        }
    }
}


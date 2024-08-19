using Domain.Constant;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Bing
{
    public interface IBingSearchService
    {
        Task<string> SearchAsync(string url, string searchPhrase, int totalResults);
    }

    public class BingSearchService : IBingSearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const int MaxItemsPerPage = 10;

        public BingSearchService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> SearchAsync(string url, string searchPhrase, int totalResults)
        {
            if (totalResults < MaxItemsPerPage)
            {
                totalResults = MaxItemsPerPage;
            }

            var totalPages = totalResults / MaxItemsPerPage;
            var htmlContent = new StringBuilder();
            for (int pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                htmlContent.Append(await SearchBingAsync(searchPhrase, pageNumber));
            }

            var positions = GetPositions(htmlContent.ToString(), url, totalResults);
            return positions.Count > 0 ? string.Join(", ", positions) : "0";
        }

        private async Task<string> SearchBingAsync(string searchPhrase, int pageNumber)
        {
            var url = $"{CommonConstant.BingUrl}/search?q={Uri.EscapeDataString(searchPhrase)}&first={pageNumber * MaxItemsPerPage + 1}";

            var httpClient = _httpClientFactory.CreateClient("BingSearch");
            httpClient.DefaultRequestHeaders.Add("User-Agent", CommonConstant.DefaultUserAgent);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private List<int> GetPositions(string html, string targetUrl, int totalResults)
        {
            var positions = new List<int>();
            var regex = new Regex(CommonConstant.BingUrlPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(html).Take(totalResults);
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


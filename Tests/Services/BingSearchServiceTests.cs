using Infrastructure.Bing;
using Moq;
using Moq.Protected;
using System.Net;

namespace Tests.Services
{
    public class BingSearchServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly BingSearchService _bingSearchService;

        public BingSearchServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _bingSearchService = new BingSearchService(_httpClientFactoryMock.Object);
        }

        private void SetupHttpResponse(string responseContent)
        {
            _httpMessageHandlerMock.Protected()
              .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                     ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                     && req.RequestUri != null),
                     ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(new HttpResponseMessage
              {
                  StatusCode = HttpStatusCode.OK,
                  Content = new StringContent(responseContent)
              });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [Fact]
        public async Task SearchAsync_MatchedUrls_ReturnsExpectedPositions()
        {
            // Arrange
            var expectedHtmlContent = "<cite>https://www.sympli.com.au\"" +
                "<cite>https://www.sympli.com.au\"";

            SetupHttpResponse(expectedHtmlContent);

            // Act
            var result = await _bingSearchService.SearchAsync("www.sympli.com.au", "e-settlements", 10);

            // Assert
            Assert.Equal("1, 2", result);
        }

        [Fact]
        public async Task SearchAsync_NoMatchingUrl_ReturnsZero()
        {
            // Arrange
            var expectedHtmlContent = "<a>https://www.sympli.com.au</a>" +
                "<a>https://www.sympli.com.au</a>";

            SetupHttpResponse(expectedHtmlContent);

            // Act
            var result = await _bingSearchService.SearchAsync("www.sympli.com.au", "e-settlements", 10);

            // Assert
            Assert.Equal("0", result);
        }
    }
}

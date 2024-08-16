using Application.Seo.Queries;
using Domain.Constant;
using Infrastructure.Bing;
using Infrastructure.MemoryCache;
using Infrastructure.Models;
using Infrastructure.Utils;
using Moq;

namespace Tests.Application.Queries
{
    public class GetSeoInfoFromBingRequestTests
    {
        private readonly Mock<IBingSearchService> _bingSearchServiceMock;
        private readonly Mock<IMemoryCacheService> _memoryCacheServiceMock;
        private readonly GetSeoInfoFromBingRequestHandler _handler;

        public GetSeoInfoFromBingRequestTests()
        {
            _bingSearchServiceMock = new Mock<IBingSearchService>();
            _memoryCacheServiceMock = new Mock<IMemoryCacheService>();
            _handler = new GetSeoInfoFromBingRequestHandler(_bingSearchServiceMock.Object, _memoryCacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedValue_WhenCacheExists()
        {
            // Arrange
            var request = new GetSeoInfoFromBingRequest { Url = "http://example.com", SearchPhrase = "example" };
            var cachedSeoInfo = new SeoInfo { Position = "1", SearchProvider = CommonConstant.Bing };

            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Bing,
                SearchPhrase = request.SearchPhrase,
                Url = request.Url
            });

            _memoryCacheServiceMock
                .Setup(x => x.TryGetValue(cacheKey, out cachedSeoInfo))
                .Returns(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(cachedSeoInfo, result);
            _bingSearchServiceMock.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFetchSeoInfoAndCacheIt_WhenCacheDoesNotExist()
        {
            // Arrange
            var request = new GetSeoInfoFromBingRequest { Url = "http://example.com", SearchPhrase = "example" };
            var seoInfo = new SeoInfo { Position = "1", SearchProvider = CommonConstant.Bing };
            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Bing,
                SearchPhrase = request.SearchPhrase,
                Url = request.Url
            });

            _memoryCacheServiceMock
                .Setup(x => x.TryGetValue(cacheKey, out It.Ref<SeoInfo>.IsAny))
                .Returns(false);

            _bingSearchServiceMock
                .Setup(x => x.SearchAsync(request.Url, request.SearchPhrase, CommonConstant.TotalResults))
                .ReturnsAsync("1");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(seoInfo.Position, result.Position);
            Assert.Equal(seoInfo.SearchProvider, result.SearchProvider);
            _memoryCacheServiceMock.Verify(x => x.Set(cacheKey, It.IsAny<SeoInfo>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}

using Application.Seo.Queries;
using Domain.Constant;
using Infrastructure.Google;
using Infrastructure.MemoryCache;
using Infrastructure.Models;
using Infrastructure.Utils;
using Moq;

namespace Tests.Application.Queries
{
    public class GetSeoInfoFromGoogleRequestTests
    {
        private readonly Mock<IGoogleSearchService> _googleSearchServiceMock;
        private readonly Mock<IMemoryCacheService> _memoryCacheServiceMock;
        private readonly GetSeoInfoFromGoogleRequestHandler _handler;

        public GetSeoInfoFromGoogleRequestTests()
        {
            _googleSearchServiceMock = new Mock<IGoogleSearchService>();
            _memoryCacheServiceMock = new Mock<IMemoryCacheService>();
            _handler = new GetSeoInfoFromGoogleRequestHandler(_googleSearchServiceMock.Object, _memoryCacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedValue_WhenCacheExists()
        {
            // Arrange
            var request = new GetSeoInfoFromGoogleRequest { Url = "http://example.com", SearchPhrase = "example" };
            var cachedSeoInfo = new SeoInfo { Position = "1", SearchProvider = CommonConstant.Google };

            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Google,
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
            _googleSearchServiceMock.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFetchSeoInfoAndCacheIt_WhenCacheDoesNotExist()
        {
            // Arrange
            var request = new GetSeoInfoFromGoogleRequest { Url = "http://example.com", SearchPhrase = "example" };
            var seoInfo = new SeoInfo { Position = "1", SearchProvider = CommonConstant.Google };
            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Google,
                SearchPhrase = request.SearchPhrase,
                Url = request.Url
            });

            _memoryCacheServiceMock
                .Setup(x => x.TryGetValue(cacheKey, out It.Ref<SeoInfo>.IsAny))
                .Returns(false);

            _googleSearchServiceMock
                .Setup(x => x.SearchAsync(request.Url, request.SearchPhrase))
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

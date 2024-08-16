using Application.Seo.Queries;
using Domain.Constant;
using SeoCheckerApi.Controllers;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controllers
{
    public class SeoControllerTests
    {
        private SeoController? _seoController;

        [Fact]
        public async Task GetSeoInfoFromGoogle_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedResult = new SeoInfo
            {
                Position = "1, 10, 33",
                SearchProvider = CommonConstant.Google
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetSeoInfoFromGoogleRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            _seoController = new SeoController(mockMediator.Object);

            var query = new GetSeoInfoFromGoogleRequest
            {
                SearchPhrase = "e-settlements",
                Url = "www.sympli.com.au"
            };

            // Act
            var response = await _seoController.GetSeoInfoFromGoogle(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);

            var seoInfoResponse = (okResult.Value as SeoInfo);
            Assert.NotNull(seoInfoResponse);
            Assert.Equal(expectedResult.Position, seoInfoResponse.Position);
            Assert.Equal(expectedResult.SearchProvider, seoInfoResponse.SearchProvider);
        }

        [Fact]
        public async Task GetSeoInfoFromBing_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedResult = new SeoInfo
            {
                Position = "1, 10, 33",
                SearchProvider = CommonConstant.Bing
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetSeoInfoFromBingRequest>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);


            _seoController = new SeoController(mockMediator.Object);

            var query = new GetSeoInfoFromBingRequest
            {
                SearchPhrase = "e-settlements",
                Url = "www.sympli.com.au"
            };

            // Act
            var response = await _seoController.GetSeoInfoFromBing(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);

            var seoInfoResponse = (okResult.Value as SeoInfo);
            Assert.NotNull(seoInfoResponse);
            Assert.Equal(expectedResult.Position, seoInfoResponse.Position);
            Assert.Equal(expectedResult.SearchProvider, seoInfoResponse.SearchProvider);
        }
    }
}


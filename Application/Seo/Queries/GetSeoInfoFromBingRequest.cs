using Domain.Constant;
using FluentValidation;
using Infrastructure.Bing;
using Infrastructure.MemoryCache;
using Infrastructure.Models;
using Infrastructure.Utils;
using MediatR;

namespace Application.Seo.Queries
{
    public class GetSeoInfoFromBingRequest : IRequest<SeoInfo>
    {
        public string Url { get; set; }
        public string SearchPhrase { get; set; }

        public class GetSeoInfoFromBingRequestValidator : AbstractValidator<GetSeoInfoFromBingRequest>
        {
            public GetSeoInfoFromBingRequestValidator()
            {
                RuleFor(c => c.Url)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Url is required.");

                RuleFor(c => c.SearchPhrase)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Search phrase is required.");

                RuleFor(c => c.SearchPhrase)
                    .Length(1, 200)
                    .WithMessage("Search phrase length can't be more than 200 characters.");
            }
        }
    }

    public class GetSeoInfoFromBingRequestHandler : IRequestHandler<GetSeoInfoFromBingRequest, SeoInfo>
    {
        private readonly IBingSearchService _bingSearchService;
        private readonly IMemoryCacheService _memoryCache;

        public GetSeoInfoFromBingRequestHandler(IBingSearchService bingSearchService, IMemoryCacheService memoryCache)
        {
            _bingSearchService = bingSearchService;
            _memoryCache = memoryCache;
        }

        public async Task<SeoInfo> Handle(GetSeoInfoFromBingRequest request,
            CancellationToken cancellationToken)
        {
            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Bing,
                SearchPhrase = request.SearchPhrase,
                Url = request.Url
            });

            if (!_memoryCache.TryGetValue(cacheKey, out SeoInfo? cachedValue))
            {
                cachedValue = new SeoInfo
                {
                    Position = await _bingSearchService.SearchAsync(request.Url, request.SearchPhrase, CommonConstant.TotalResults),
                    SearchProvider = CommonConstant.Bing,
                };

                _memoryCache.Set(cacheKey, cachedValue, TimeSpan.FromHours(1));

                return cachedValue;
            }

            return cachedValue ?? new SeoInfo
            {
                Position = "0",
                SearchProvider = CommonConstant.Bing
            };
        }
    }
}

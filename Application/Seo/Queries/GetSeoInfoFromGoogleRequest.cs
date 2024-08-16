using Domain.Constant;
using FluentValidation;
using Infrastructure.Google;
using Infrastructure.MemoryCache;
using Infrastructure.Models;
using Infrastructure.Utils;
using MediatR;

namespace Application.Seo.Queries
{
    public class GetSeoInfoFromGoogleRequest : IRequest<SeoInfo>
    {
        public string Url { get; set; }
        public string SearchPhrase { get; set; }

        public class GetSeoInfoFromGoogleRequestValidator : AbstractValidator<GetSeoInfoFromGoogleRequest>
        {
            public GetSeoInfoFromGoogleRequestValidator()
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

    public class GetSeoInfoFromGoogleRequestHandler : IRequestHandler<GetSeoInfoFromGoogleRequest, SeoInfo>
    {
        private readonly IGoogleSearchService _googleSearchService;
        private readonly IMemoryCacheService _memoryCache;

        public GetSeoInfoFromGoogleRequestHandler(IGoogleSearchService googleSearchService, IMemoryCacheService memoryCache)
        {
            _googleSearchService = googleSearchService;
            _memoryCache = memoryCache;
        }

        public async Task<SeoInfo> Handle(GetSeoInfoFromGoogleRequest request,
            CancellationToken cancellationToken)
        {
            var cacheKey = MemoryCacheUtil.GetCacheKey(new SeoRequest
            {
                SearchProvider = CommonConstant.Google,
                SearchPhrase = request.SearchPhrase,
                Url = request.Url
            });

            if (!_memoryCache.TryGetValue(cacheKey, out SeoInfo? cachedValue))
            {
                cachedValue = new SeoInfo
                {
                    Position = await _googleSearchService.SearchAsync(request.Url, request.SearchPhrase),
                    SearchProvider = CommonConstant.Google,
                };

                _memoryCache.Set(cacheKey, cachedValue, TimeSpan.FromHours(1));

                return cachedValue;
            }

            return cachedValue ?? new SeoInfo
            {
                Position = "0",
                SearchProvider = CommonConstant.Google,
            };
        }
    }
}

using Application.Seo.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SeoCheckerApi.Controllers
{
    [ApiController]
    [Route("seo")]
    public class SeoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("google")]
        public async Task<IActionResult> GetSeoInfoFromGoogle([FromQuery][Required] GetSeoInfoFromGoogleRequest getSeoInfoFromGoogleRequest)
        {
            return Ok(await _mediator.Send(getSeoInfoFromGoogleRequest));
        }

        [HttpGet]
        [Route("bing")]
        public async Task<IActionResult> GetSeoInfoFromBing([FromQuery][Required] GetSeoInfoFromBingRequest getSeoInfoFromBingRequest)
        {
            return Ok(await _mediator.Send(getSeoInfoFromBingRequest));
        }
    }
}

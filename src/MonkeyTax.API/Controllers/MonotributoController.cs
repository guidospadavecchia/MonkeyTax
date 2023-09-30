using Microsoft.AspNetCore.Mvc;
using MonkeyTax.API.Routing.Model;
using MonkeyTax.Application.Monotributo.Model;
using MonkeyTax.Application.Monotributo.Services.Monotributo;
using System.Net;

namespace MonkeyTax.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonotributoController : ControllerBase
    {
        private readonly IMonotributoService _monotributoService;

        public MonotributoController(IMonotributoService monotributoService)
        {
            _monotributoService = monotributoService;
        }

        /// <summary>
        /// Obtiene todos los montos y categorías vigentes.
        /// </summary>
        [HttpGet("categorias")]
        [ProducesResponseType(typeof(MonotributoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCategoriasAsync([FromHeader(Name = "Cache-Control")] string? cacheControl, CancellationToken cancellationToken = default)
        {            
            return Ok(await _monotributoService.GetValuesAsync(cacheControl, cancellationToken));
        }
    }
}

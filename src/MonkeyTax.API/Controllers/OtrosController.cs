using Microsoft.AspNetCore.Mvc;
using MonkeyTax.API.Routing.Model;
using System.Net;

namespace MonkeyTax.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        /// <summary>
        /// Endpoint para corroborar el estado de la aplicación.
        /// </summary>
        [HttpGet("live")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetStatusAsync()
        {
            return Ok(HttpStatusCode.OK.ToString());
        }
    }
}

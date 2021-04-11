using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRecordController : ControllerBase
    {
        private readonly ILogger<CustomerRecordController> _logger;

        public CustomerRecordController(ILogger<CustomerRecordController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Метод возвращает все справочники
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>List of Dictionaries</returns>
        /// <response code="200">Request is created</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Not Founded</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet("all")]
        public async Task<ActionResult> GetAllDictionaries()
        {
            return StatusCode((int)HttpStatusCode.OK, "scasc");
        }



    }
}

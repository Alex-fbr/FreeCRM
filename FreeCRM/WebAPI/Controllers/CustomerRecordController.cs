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

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet("Ok")]
        public async Task<ActionResult> GetOk()
        {

            return StatusCode((int)HttpStatusCode.OK, "Все хорошо");
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet("InternalServerError")]
        public async Task<ActionResult> GetInternalServerError()
        {

            return StatusCode((int)HttpStatusCode.InternalServerError, "Сервис вернул InternalServerError");
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet("Exception")]
        public async Task<ActionResult> GetException()
        {
            throw new System.Exception("Сервис выкинул Exception");
        }
    }
}

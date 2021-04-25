using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet()]
        public async Task<ActionResult> GetOk()
        {

            return StatusCode((int)HttpStatusCode.OK, "Читаем настройки");
        }

        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost()]
        public async Task<ActionResult> SaveSettings()
        {

            return StatusCode((int)HttpStatusCode.Created, "Сохраняем настройки");
        }

    }
}

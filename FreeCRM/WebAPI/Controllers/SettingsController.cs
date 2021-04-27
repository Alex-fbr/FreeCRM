using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Common.CommandsSettingsXml;

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
            var XMLFileName = Environment.CurrentDirectory + "\\settings.xml";

            if (System.IO.File.Exists(XMLFileName))
            {
                var ser = new XmlSerializer(typeof(CommandsSettingsXml));
                using var reader = new StreamReader(XMLFileName);
                var settings = ser.Deserialize(reader) as CommandsSettingsXml;
                reader.Close();
                return StatusCode((int)HttpStatusCode.OK, settings);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "файл настроек не найден");
        }

        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost()]
        public async Task<ActionResult> SaveSettings()
        {
            var Fields = new CommandsSettingsXml();

            Fields.BotCommandList.Add(new BotCommandXml()
            {
                Type = CommandType.InlineKeyboard,
                Code = "/inline",
                Description = "пришлет список команд",
                KeyboardButtonList = new List<List<Keyboard>>()
                {
                    new  List<Keyboard>()
                    {
                        new Keyboard(){ DisplayToUser = "Прислать кнопки", CallbackData = "/keyboard"},
                    },
                    new  List<Keyboard>()
                    {
                        new Keyboard(){ DisplayToUser = "Прислать фото разработчика", CallbackData = "/developerPhoto"},
                    },
                    new  List<Keyboard>()
                    {
                        new Keyboard(){ DisplayToUser = "Запросить контактные данные", CallbackData = "/request"},
                    },
                },
            });

            Fields.BotCommandList.Add(new BotCommandXml()
            {
                Type = CommandType.ReplyKeyboard,
                Code = "/keyboard",
                Description = "пришлет кнопки",
                KeyboardButtonList = new List<List<Keyboard>>()
                {
                    new  List<Keyboard>()
                    {
                        new Keyboard(){ DisplayToUser = "Акции"},
                        new Keyboard(){ DisplayToUser = "Облигации"},
                        new Keyboard(){ DisplayToUser = "Фонды" },
                    }
                },
            });

            Fields.BotCommandList.Add(new BotCommandXml()
            {
                Type = CommandType.GetPhoto,
                Code = "/developerPhoto",
                Description = "отправит фото разработчика бота",
            });


            Fields.BotCommandList.Add(new BotCommandXml()
            {
                Type = CommandType.Request,
                Code = "/request",
                Description = "пришлет запрос контактов",
            });

            var XMLFileName = Environment.CurrentDirectory + "\\settings.xml";
            var ser = new XmlSerializer(typeof(CommandsSettingsXml));
            using var writer = new StreamWriter(XMLFileName);
            ser.Serialize(writer, Fields);
            writer.Close();

            return StatusCode((int)HttpStatusCode.Created, "Сохраняем настройки");
        }

    }
}

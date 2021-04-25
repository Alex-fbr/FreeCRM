using System;
using System.Xml.Serialization;

namespace Common
{
    /// <summary>
    /// Настройки для хранения команд бота
    /// </summary>
    [XmlRoot]
    [Serializable]
    public class CommandsSettingsXml
    {
        [XmlArray]
        [XmlArrayItem(typeof(BotCommandXml), ElementName = nameof(BotCommandXml))]
        public BotCommandXml[] BotCommandList { get; set; }

        //[XmlElement]
        //public CommonRequestSupervisingSettings CommonRequestSupervisingSettings { get; set; }

        //[XmlAttribute]
        //public string TimeOffsetSettingsName { get; set; } = "TimeOffset";

        //[XmlElement]
        //public TimeOffsetType DefaultTimeOffsetManager { get; set; } = TimeOffsetType.Workplace;

        //[XmlElement]
        //public string PersonalizationSettingsName { get; set; }

        public CommandsSettingsXml()
        {
        }

        public class BotCommandXml
        {

        }
    }
}

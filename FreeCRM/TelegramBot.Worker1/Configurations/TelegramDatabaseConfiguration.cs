
using static Common.DatabaseEnums;

namespace TelegramBot.Worker.Configurations
{
    public class TelegramDatabaseConfiguration
    {
        public string DatabaseStringType { get; set; }
        public string ConnectionStringName { get; set; }
        public DatabaseTypes? DatabaseType => !string.IsNullOrEmpty(DatabaseStringType) ? Enum.Parse<DatabaseTypes>(DatabaseStringType) : null;
    }
}

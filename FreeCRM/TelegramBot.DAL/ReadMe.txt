
Add-Migration InitialCreate -Context TelegramMsSqlDbContext -OutputDir Migrations\MsSqlMigrations
Add-Migration InitialCreate -Context TelegramPostgresDbContext -OutputDir Migrations\PostgresMigrations

 Update-Database -Context TelegramMsSqlDbContext
﻿Add-Migration InitialCreate -Context TelegramPostgresDbContext -OutputDir Migrations\Postgres
Update-Database -Context TelegramPostgresDbContext
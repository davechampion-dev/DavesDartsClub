dotnet ef migrations add 1_Initial  --project ./DavesDartsClub.Infrastructure --startup-project ./DavesDartsClub.Aspire.AppHost --output-dir ./EntityFramework/Migrations

dotnet ef migrations script --output ./DavesDartsClub.Infrastructure/EntityFramework/Migrations/TSQL/1_Initial.sql --project ./DavesDartsClub.Infrastructure --startup-project ./DavesDartsClub.Aspire.AppHost
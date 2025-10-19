dotnet ef migrations add 1_Initial  --project ./DavesDartsClub.Infrastructure --startup-project ./DavesDartsClub.Aspire.AppHost --output-dir ./EntityFramework/Migrations

dotnet ef migrations script --output ./DavesDartsClub.Infrastructure/EntityFramework/Migrations/TSQL/1_Initial.sql --project ./DavesDartsClub.Infrastructure --startup-project ./DavesDartsClub.Aspire.AppHost

//Subsequent runs 

dotnet ef migrations add 002_additionaltables --context AppDbContext --project "./DavesDartsClub.Infrastructure/DavesDartsClub.Infrastructure.csproj" --startup-project "./DavesDartsClub.Aspire.AppHost/DavesDartsClub.Aspire.AppHost.csproj"

dotnet ef migrations script --idempotent --output "./DavesDartsClub.Infrastructure/EntityFramework/Migrations/TSQL/002_aditionaltables.sql" --context AppDbContext --project "./DavesDartsClub.Infrastructure/DavesDartsClub.Infrastructure.csproj" --startup-project "./DavesDartsClub.Aspire.AppHost/DavesDartsClub.Aspire.AppHost.csproj"
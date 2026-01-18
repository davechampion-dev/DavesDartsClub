// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.


[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Analyzer missunderstanding type intent changes when adding ConfigureAwait(false)", Scope = "member", Target = "~M:DavesDartsClub.DatabaseMigrationService.Worker.SeedDataAsync(DavesDartsClub.Infrastructure.EntityFramework.AppDbContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task")]

using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using DavesDartsClub.Fakers;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DavesDartsClub.Infrastructure.Seeding
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAllAsync(CancellationToken ct = default)
        {
            await SeedLeaguesAsync(ct);
            await SeedMembersAsync(ct);
            await SeedPlayersAsync(ct);
        }

        private async Task SeedLeaguesAsync(CancellationToken ct = default)
        {
            if (await _context.Leagues.AnyAsync(ct))
                return;

            var leagueFaker = new LeagueFaker();
            var leagues = leagueFaker.CreateFaker().Generate(5); // List<League>

            var leagueEntities = leagues.Select(l => new LeagueEntity
            {
                LeagueId = Guid.NewGuid(), // EF primary key
                LeagueName = l.LeagueName
            }).ToList();

            _context.Leagues.AddRange(leagueEntities);
            await _context.SaveChangesAsync(ct);
        }

        private async Task SeedMembersAsync(CancellationToken ct = default)
        {
            if (await _context.Set<Member>().AnyAsync(ct))
                return;

            var memberFaker = new MemberFaker();
            var members = memberFaker.GenerateMany(50);
            _context.Set<Member>().AddRange(members);
            await _context.SaveChangesAsync(ct);
        }

        private async Task SeedPlayersAsync(CancellationToken ct = default)
        {
            if (await _context.Set<PlayerProfile>().AnyAsync(ct))
                return;

            var playerFaker = new PlayerFaker();
            var players = playerFaker.GenerateMany(100);
            _context.Set<PlayerProfile>().AddRange(players);
            await _context.SaveChangesAsync(ct);
        }
    }
}


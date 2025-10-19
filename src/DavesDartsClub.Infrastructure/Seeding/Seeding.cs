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
            if (await _context.Set<MemberEntity>().AnyAsync(ct)) return;

            var memberFaker = new MemberFaker();
            var members = memberFaker.CreateFaker().Generate(5); // List<Member> domain objects

            var memberEntities = members.Select(m => new MemberEntity
            {
                MemberId = Guid.NewGuid(),
                MemberName = m.MemberName
                // Map other properties here
            }).ToList();

            _context.Members.AddRange(memberEntities);
            await _context.SaveChangesAsync(ct);
        }

        private async Task SeedPlayersAsync(CancellationToken ct = default)
        {
            if (await _context.PlayerProfiles.AnyAsync(ct)) // ✅ plural matches AppDbContext
                return;

            var playerFaker = new PlayerFaker();
            var players = playerFaker.GenerateMany(100); // List<PlayerProfile> domain objects

            var playerEntities = players.Select(p => new PlayerProfileEntity
            {
                PlayerId = Guid.NewGuid(),      // EF primary key
                MemberId = p.MemberId,          // map from domain object
                Nickname = p.Nickname
            }).ToList();

            _context.PlayerProfiles.AddRange(playerEntities);
            await _context.SaveChangesAsync(ct);
        }
    }
}


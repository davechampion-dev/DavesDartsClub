namespace DavesDartsClub.Application;

// This is the "League Mathematician"
public class FixtureGenerator
{
    // This is the secret formula (Round-Robin) to make sure everyone plays everyone
    public List<(Guid HomeTeamId, Guid AwayTeamId)> GenerateRoundRobin(IEnumerable<Guid> teamIds)
    {
        var teams = new List<Guid>(teamIds);

        // odd number team gets a "Bye"
        if (teams.Count % 2 != 0)      
            teams.Add(Guid.Empty);

        var numTeams = teams.Count;
        var numDays = numTeams - 1;
        var halfSize = numTeams / 2;

        var fixtures = new List<(Guid, Guid)>();

        // The "Circle Method": Rotate the teams like a dial to create matches
        for (int day = 0; day < numDays; day++)
        {
            for (int i = 0; i < halfSize; i++)
            {
                var home = teams[i];
                var away = teams[numTeams - 1 - i];

                // Don't create a match if it's against the "Ghost" (Bye) team
                if (home != Guid.Empty && away != Guid.Empty)
                {
                    fixtures.Add((home, away));
                }
            }

            // Rotate the teams for the next day of play
            var lastTeam = teams[^1];
            teams.RemoveAt(teams.Count - 1);
            teams.Insert(1, lastTeam);
        }

        return fixtures;
    }
}
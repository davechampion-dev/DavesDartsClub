namespace DavesDartsClub.Application;

// This is the "League Mathematician"
public class FixtureGenerator
{
    // This is the secret formula (Round-Robin) to make sure everyone plays everyone
    public List<(Guid HomeTeamId, Guid AwayTeamId)> GenerateRoundRobin(List<Guid> teamIds)
    {
        // 1. If we have an odd number of teams, we need a "Bye" (a ghost team)
        if (teamIds.Count % 2 != 0)
        {
            teamIds.Add(Guid.Empty);
        }

        int numTeams = teamIds.Count;
        int numDays = numTeams - 1;
        int halfSize = numTeams / 2;

        var fixtures = new List<(Guid, Guid)>();
        var teams = new List<Guid>(teamIds);

        // 2. The "Circle Method": Rotate the teams like a dial to create matches
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
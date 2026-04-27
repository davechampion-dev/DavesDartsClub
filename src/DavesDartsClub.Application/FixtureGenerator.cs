namespace DavesDartsClub.Application;

public class FixtureGenerator
{
    public List<(Guid HomeTeamId, Guid AwayTeamId)> GenerateRoundRobin(IEnumerable<Guid> teamIds)
    {
        var teams = new List<Guid>(teamIds);
        if (teams.Count % 2 != 0)      
            teams.Add(Guid.Empty);

        var numTeams = teams.Count;
        var numDays = numTeams - 1;
        var halfSize = numTeams / 2;

        var fixtures = new List<(Guid, Guid)>();

        for (int day = 0; day < numDays; day++)
        {
            for (int i = 0; i < halfSize; i++)
            {
                var home = teams[i];
                var away = teams[numTeams - 1 - i];

                if (home != Guid.Empty && away != Guid.Empty)
                {
                    fixtures.Add((home, away));
                }
            }

            var lastTeam = teams[^1];
            teams.RemoveAt(teams.Count - 1);
            teams.Insert(1, lastTeam);
        }

        return fixtures;
    }
}
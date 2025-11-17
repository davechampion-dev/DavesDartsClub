namespace DavesDartsClub.Website.ApiClient;

using DavesDartsClub.SharedContracts.League;
using DavesDartsClub.SharedContracts.Player;
using DavesDartsClub.SharedContracts.Tournament;
using DavesDartsClub.SharedContracts.Member;
using Refit;

//ToDo: Make controller Async 
[Headers("Accept: application/json")]
public interface ILeagueApiClient
{
    [Post("/League")]
    Task<ApiResponse<LeagueResponse>> CreateLeague([Body] LeagueRequest leagueRequest);

    [Delete("/League/{LeagueId}")]
    Task<ApiResponse<object>> DeleteLeague(Guid leagueId);

    [Get("/League/{LeagueId}")]
    Task<ApiResponse<LeagueResponse>> GetLeagueById(Guid leagueId);

    [Get("/League/search")]
    Task<ApiResponse<IEnumerable<LeagueResponse>>> GetLeagueSearch([AliasAs("leagueName")] string leagueName);
}

public interface IMemberApiClient
{
    [Post("/Member")]
    Task<ApiResponse<MemberResponse>> CreateMember([Body] MemberRequest memberRequest);

    [Delete("/Member/{MemberId}")]
    Task<ApiResponse<object>> DeleteMember(Guid memberId);

    [Get("/Member/{MemberId}")]
    Task<ApiResponse<MemberResponse>> GetMemberById(Guid memberId);

    [Get("/Member/search")]
    Task<ApiResponse<IEnumerable<MemberResponse>>> MemberSearch([AliasAs("memberName")] string memberName);
}

public interface IPlayerApiClient
{
    [Post("/Player")]
    Task<ApiResponse<PlayerResponse>> CreatePlayer([Body] PlayerRequest playerRequest);

    [Delete("/Player/{MemberId}")]
    Task<ApiResponse<object>> DeletePlayer(Guid memberId);

    [Get("/Player/{MemberId}")]
    Task<ApiResponse<PlayerResponse>> GetPlayerByMemberId(Guid memberId);

    [Get("/Player/search")]
    Task<ApiResponse<IEnumerable<PlayerResponse>>> GetPlayerSearch([AliasAs("playerName")] string playerName);
}

public interface ITournamentApiClient
{
    [Post("/Tournament")]
    Task<ApiResponse<TournamentResponse>> CreateTournament([Body] TournamentRequest tournamentRequest);

    [Delete("/Tournament/{tournamentId}")]
    Task<ApiResponse<object>> DeleteTournament(Guid tournamentId);

    [Get("/Tournament/{tournamentId}")]
    Task<ApiResponse<TournamentResponse>> GetTournamentById(Guid tournamentId);

    [Get("/Tournament/search")]
    Task<ApiResponse<IEnumerable<TournamentResponse>>> GetTournamentSearch([AliasAs("tournamentName")] string tournamentName);
}






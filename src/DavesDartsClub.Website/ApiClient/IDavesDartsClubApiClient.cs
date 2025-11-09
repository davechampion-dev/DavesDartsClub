namespace DavesDartsClub.Website.ApiClient;

using DavesDartsClub.SharedContracts.Tournament;
using Refit;

//todo: Make controller Async 
[Headers("Accept: application/json")]
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

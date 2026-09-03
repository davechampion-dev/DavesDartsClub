using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Venue;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpPost(Name = nameof(CreateVenue))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateVenue([FromBody] VenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            VenueName = venueRequest.VenueName,
            Address = venueRequest.Address,
            City = venueRequest.City,
            Postcode = venueRequest.Postcode,
            ContactPhone = venueRequest.ContactPhone,
            ContactEmail = venueRequest.ContactEmail,
            NumberOfBoards = venueRequest.NumberOfBoards,
            IsActive = venueRequest.IsActive
        };


        var venueResult = await _venueService.CreateVenueAsync(venue, cancellationToken).ConfigureAwait(false);

        if (venueResult.Status != ResultStatus.Created)
        {
            return BadRequest(venueResult.Errors);
        }

        return CreatedAtRoute(nameof(GetVenueById), new { venueId = venueResult.Value.VenueId }, venueResult.Value.VenueId);
    }

    [HttpGet("{venueId}", Name = nameof(GetVenueById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<VenueResponse>> GetVenueById(Guid venueId, CancellationToken cancellationToken)
    {
        var venue = await _venueService.GetVenueByIdAsync(venueId, cancellationToken).ConfigureAwait(false);

        if (venue == null)
        {
            return NotFound();
        }

        var result = new VenueResponse
        {
            VenueId = venue.VenueId,
            VenueName = venue.VenueName,
            Address = venue.Address,
            City = venue.City,
            Postcode = venue.Postcode,
            ContactPhone = venue.ContactPhone,
            ContactEmail = venue.ContactEmail,
            NumberOfBoards = venue.NumberOfBoards,
            IsActive = venue.IsActive,
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostVenueSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<VenueResponse>>> PostVenueSearch([NotNull, FromBody] VenueSearchRequest venueName, CancellationToken cancellationToken)
    {
        var venues = await _venueService.GetVenueByNameAsync(venueName.VenueName, cancellationToken);

        var results = venues.Select(v => new VenueResponse

        {
            VenueId = v.VenueId,
            VenueName = v.VenueName,
            Address = v.Address,
            City = v.City,
            Postcode = v.Postcode,
            ContactPhone = v.ContactPhone,
            ContactEmail = v.ContactEmail,
            NumberOfBoards = v.NumberOfBoards,
            IsActive = v.IsActive
        }).ToList();

        return Ok(results);
    }

    [HttpDelete("{venueId}", Name = nameof(DeleteVenue))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteVenue(Guid venueId, CancellationToken cancellationToken)
    {
        var venueExists = true;
        if (!venueExists)
        {
            return NotFound();
        }
        return NoContent();
    }
}

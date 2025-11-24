using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/[controller]")]
[Authorize(Roles = "HR")] // restrict for now; can open candidate view later
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultOfferDto>> GetOffers([FromQuery] OfferFilterDto filter)
    {
        var result = await _offerService.GetOffersAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OfferLetterDto>> GetOffer(int id)
    {
        var offer = await _offerService.GetOfferAsync(id);
        if (offer == null) return NotFound();
        return Ok(offer);
    }

    [HttpPost]
    public async Task<ActionResult<OfferLetterDto>> CreateOffer([FromBody] CreateOfferLetterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _offerService.CreateOfferAsync(dto);
        return CreatedAtAction(nameof(GetOffer), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OfferLetterDto>> UpdateOffer(int id, [FromBody] UpdateOfferLetterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _offerService.UpdateOfferAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOffer(int id)
    {
        var ok = await _offerService.DeleteOfferAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}

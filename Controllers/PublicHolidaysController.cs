using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class PublicHolidaysController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PublicHolidaysController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PublicHoliday>>> GetPublicHolidays()
    {
        var holidays = await _context.PublicHolidays
            .OrderBy(h => h.Date)
            .ToListAsync();
        return Ok(holidays);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Only admins can manage holidays
    public async Task<ActionResult<PublicHoliday>> CreatePublicHoliday(PublicHoliday holiday)
    {
        _context.PublicHolidays.Add(holiday);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPublicHolidays), new { id = holiday.Id }, holiday);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PublicHoliday>> UpdatePublicHoliday(Guid id, PublicHoliday holiday)
    {
        if (id != holiday.Id)
        {
            return BadRequest();
        }

        var existingHoliday = await _context.PublicHolidays.FindAsync(id);
        if (existingHoliday == null)
        {
            return NotFound();
        }

        existingHoliday.Date = holiday.Date;
        existingHoliday.Name = holiday.Name;
        existingHoliday.IsRecurring = holiday.IsRecurring;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await HolidayExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return Ok(existingHoliday);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePublicHoliday(Guid id)
    {
        var holiday = await _context.PublicHolidays.FindAsync(id);
        if (holiday == null)
        {
            return NotFound();
        }

        _context.PublicHolidays.Remove(holiday);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> HolidayExists(Guid id)
    {
        return await _context.PublicHolidays.AnyAsync(h => h.Id == id);
    }
}
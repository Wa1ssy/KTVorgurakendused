using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly DataContext _context;

    public TicketsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Ticket>> GetTickets(string? name = null)
    {
        var query = _context.Tickets!.AsQueryable();

        if (name != null)
            query = query.Where(x => x.SeatNo != null && x.SeatNo.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetTicket(int id)
    {
        var tick = _context.Tickets!.Find(id);

        if (tick == null)
        {
            return BadRequest();
        }

        return Ok(tick);
    }

    [HttpPut("{id}")]
    public IActionResult PutTicket(int id, Ticket tick)
    {
        var dbTest = _context.Tickets!.AsNoTracking().FirstOrDefault(x => x.Id == tick.Id);
        if (id != tick.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(tick);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Ticket> PostTicket(Ticket tick)
    {
        var dbExercise = _context.Tests!.Find(tick.Id);
        if (dbExercise == null)
        {
            _context.Add(tick);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTicket), new { Id = tick.Id }, tick);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id)
    {
        var tick = _context.Tickets!.Find(id);
        if (tick == null)
        {
            return NotFound();
        }

        _context.Remove(tick);
        _context.SaveChanges();

        return NoContent();
    }
}

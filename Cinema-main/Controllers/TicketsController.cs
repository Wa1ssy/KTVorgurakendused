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
    public ActionResult GetTickets()
    {

        return Ok(_context.Tickets);
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetTicket(int id)
    {
        var ses = _context.Tickets?.Find(id);

        if (ses == null)
        {
            return NotFound();
        }
        return Ok(ses);
    }

    [HttpPut("{id}")]
    public IActionResult ChangeSession(int id, Test test)
    {
        var dbTest = _context.Tests!.AsNoTracking().FirstOrDefault(x => x.Id == test.Id);
        if (id != test.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(test);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Movie> AddTicket(Ticket ticket)
    {
        if (ticket == null)
        {
            return BadRequest();
        }
        if (ticket.Price < 0)
        {
            return BadRequest();
        }
        var sessionExists = _context.Sessions?.Where(s=>s.Id == ticket.SessionId);
        if(sessionExists == null)
        {
            return BadRequest();
        }

        bool isNotUnique = _context.Tickets.Any(t=>t.SeatNo == ticket.SeatNo);
        if (isNotUnique)
        {
            return BadRequest();
        }
        
        _context.Tickets?.Add(ticket);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetTicket), new { Id = ticket.Id }, ticket); ;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTicket(int id)
    {
        var ses = _context.Tickets.Find(id);
        if (ses == null)
        {
            return NotFound();
        }

        _context.Remove(ses);
        _context.SaveChanges();

        return Ok();
    }
}

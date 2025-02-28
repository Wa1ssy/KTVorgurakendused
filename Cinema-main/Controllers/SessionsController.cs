using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Sockets;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly DataContext _context;

    public SessionsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult GetSessions([FromQuery] string? periodStart, [FromQuery] string? periodEnd, [FromQuery] string? movieTitle)
    {
        
        if(periodStart != null && periodEnd != null)
        {
            return Ok(_context.Sessions.Where(s=>s.StartTime >= DateTime.Parse(periodStart.Replace("-", "/")) && s.StartTime <= DateTime.Parse(periodEnd.Replace("-", "/"))));
        }
        if(periodStart != null)
        {
            return Ok(_context.Sessions.Where(s => s.StartTime >= DateTime.Parse(periodStart.Replace("-", "/"))));
        }
        if(periodEnd != null)
        {
            return Ok(_context.Sessions.Where(s => s.StartTime <= DateTime.Parse(periodEnd.Replace("-", "/"))));
        }
        if(movieTitle != null)
        {
            return Ok(_context.Sessions.Where(s => _context.Movies.FirstOrDefault(m=>m.Id == s.MovieId).Title == movieTitle));
        }
        return Ok(_context.Sessions);
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSession(int id)
    {

        var ses = _context.Sessions?.Find(id);

        if (ses == null)
        {
            return NotFound();
        }
        return Ok(ses);
    }

    [HttpGet("{id}/tickets")]
    public ActionResult<TextReader> GetSessionTickets(int id)
    {

        var ses = _context.Sessions?.Find(id);

        if (ses == null)
        {
            return NotFound();
        }
        return Ok(_context.Tickets.Where(t=>t.SessionId==id));
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
    public ActionResult AddSession(Session session)
    {
        if (session == null)
        {
            return BadRequest();
        }
        if(DateTime.Compare(session.StartTime, DateTime.Now) < 0)
        {
            return BadRequest();
        }
        var IsAdded = _context.Sessions?.Find(session.Id);
        if (IsAdded != null)
        {
            return BadRequest();
        }
        var movieExists = _context.Movies?.Find(session.MovieId);
        if (movieExists == null)
        {
            return BadRequest();
        }

        _context.Sessions?.Add(session);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetSession), new { Id = session.Id }, session); ;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSessions(int id)
    {
        var ses = _context.Sessions.Find(id);
        if (ses == null)
        {
            return NotFound();
        }

        _context.Remove(ses);
        _context.SaveChanges();

        return Ok();
    }
}

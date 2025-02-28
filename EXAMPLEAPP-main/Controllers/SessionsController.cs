using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public ActionResult<IEnumerable<Session>> GetSessions(string? name = null)
    {

        var query = _context.Sessions!.AsQueryable();

        if (name != null)
            query = query.Where(x => x.AuditoriumName != null && x.AuditoriumName.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetSession(int id)
    {
        var sess = _context.Sessions!.Find(id);

        if (sess == null)
        {
            return BadRequest();
        }

        return Ok(sess);
    }

    [HttpPut("{id}")]
    public IActionResult PutSession(int id, Session sess)
    {
        var dbTest = _context.Sessions!.AsNoTracking().FirstOrDefault(x => x.Id == sess.Id);
        if (id != sess.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(sess);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Session> PostSession(Session sess)
    {
        var dbExercise = _context.Sessions!.Find(sess.Id);
        if (dbExercise == null)
        {
            _context.Add(sess);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSession), new { Id = sess.Id }, sess);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSession(int id)
    {
        var sess = _context.Sessions!.Find(id);
        if (sess == null)
        {
            return NotFound();
        }

        _context.Remove(sess);
        _context.SaveChanges();

        return NoContent();
    }
}

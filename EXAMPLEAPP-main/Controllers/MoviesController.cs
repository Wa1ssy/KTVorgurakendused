using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly DataContext _context;

    public MoviesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Movie>> GetMovies(string? name = null)
    {
        var query = _context.Movies!.AsQueryable();

        if (name != null)
            query = query.Where(x => x.Title != null && x.Title.ToUpper().Contains(name.ToUpper()));

        return query.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetMovie(int id)
    {
        var movi = _context.Movies!.Find(id);

        if (movi == null)
        {
            return NotFound();
        }

        return Ok(movi);
    }

    [HttpPut("{id}")]
    public IActionResult PutMovie(int id, Movie movi)
    {
        var dbTest = _context.Movies!.AsNoTracking().FirstOrDefault(x => x.Id == movi.Id);
        if (id != movi.Id || dbTest == null)
        {
            return NotFound();
        }

        _context.Update(movi);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    public ActionResult<Movie> PostMovie(Movie movi)
    {
        var dbExercise = _context.Movies!.Find(movi.Id);
        if (dbExercise == null)
        {
            _context.Add(movi);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMovie), new { Id = movi.Id }, movi);
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var movi = _context.Movies!.Find(id);
        if (movi == null)
        {
            return NotFound();
        }

        _context.Remove(movi);
        _context.SaveChanges();

        return NoContent();
    }
}

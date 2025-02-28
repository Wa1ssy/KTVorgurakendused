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
    public ActionResult GetMovies()
    {
        return Ok(_context.Movies);
    }

    [HttpGet("{id}")]
    public ActionResult<TextReader> GetMovie(int id)
    {
        var movie = _context.Movies?.Find(id);

        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [HttpPut("{id}")]
    public IActionResult ChangeMovie(int id, Test test)
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
    public ActionResult<Movie> AddMovie(Movie movie)
    {
        if(movie == null)
        {
            return BadRequest();
        }
        var IsAdded = _context.Movies?.Find(movie.Id);
        if (IsAdded != null)
        {
            return BadRequest();
        }
        _context.Movies?.Add(movie);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetMovie), new { Id = movie.Id }, movie); ;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var movie = _context.Movies.Find(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Remove(movie);
        _context.SaveChanges();

        return Ok();
    }
}

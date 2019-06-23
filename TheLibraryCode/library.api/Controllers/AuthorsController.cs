using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using library.api.Data;
using library.api.Models;
using library.api.Helpers;

namespace library.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryContext _context;

        public AuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthor()
        {
            var author = await _context.Author.ToListAsync();
            var authorDtos = author
                .Select(author => new AuthorDto
            {
                Id = author.Id,
                Name = $"{author.FirstName} {author.LastName}",
                Genre = author.Genre,
                Age = author.DateOfBirth.GetCurrentAge()
            }).ToList();

            return authorDtos;
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id)
        {
            var author = await _context.Author.FindAsync(id);

            if (author == null)
                return NotFound();

            var authorDto = new AuthorDto()
            {
                Id = author.Id,
                Name = $"{author.FirstName}, {author.LastName}",
                Genre = author.Genre,
                Age = author.DateOfBirth.GetCurrentAge()
            };

            return authorDto;
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(Guid id, Author author)
        {
            if (id != author.Id)
                return BadRequest();

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor(Author author)
        {
            _context.Author.Add(author);
            await _context.SaveChangesAsync();

            var authorDto = new AuthorDto()
            {
                Id = author.Id,
                Name = $"{author.FirstName} {author.LastName}",
                Genre = author.Genre,
                Age = author.DateOfBirth.GetCurrentAge()
            };

            return CreatedAtAction("GetAuthor", new { id = authorDto.Id }, authorDto);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Author>> DeleteAuthor(Guid id)
        {
            var author = await _context.Author.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Author.Remove(author);
            await _context.SaveChangesAsync();

            return author;
        }

        private bool AuthorExists(Guid id)
        {
            return _context.Author.Any(e => e.Id == id);
        }
    }

    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}

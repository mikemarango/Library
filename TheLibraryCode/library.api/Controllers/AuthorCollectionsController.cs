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
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly LibraryContext _context;

        public AuthorCollectionsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet("{ids}")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthor(IEnumerable<Guid> ids)
        {
            var authors = await _context.Author.Where(a => ids.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName).ToListAsync();

            var authorDtos = authors
                .Select(author => new AuthorDto
                {
                    Id = author.Id,
                    Name = $"{author.FirstName} {author.LastName}",
                    Genre = author.Genre,
                    Age = author.DateOfBirth.GetCurrentAge()
                }).ToList();

            return authorDtos;
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthors(IEnumerable<AuthorCreateDto> authorCreateDtos)
        {
            if (authorCreateDtos == null)
                return BadRequest();

            List<Author> authors = new List<Author>();

            // map authorCreateDtos --> authors
            foreach (var item in authorCreateDtos)
            {
                authors.Add(new Author
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Genre = item.Genre,
                    DateOfBirth = item.DateOfBirth
                });
            }

            // add each element to the context store
            foreach (var author in authors)
            {
                _context.Author.Add(author);
            }

            // save each element
            await _context.SaveChangesAsync();

            var authorDtos = new List<AuthorDto>();

            //map authors --> authorDtos
            foreach (var item in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    Id = item.Id,
                    Name = $"{item.FirstName} {item.LastName}",
                    Age = item.DateOfBirth.GetCurrentAge(),
                    Genre = item.Genre
                });
            }

            return Ok();
        }

        public bool AuthorExists(Guid id)
        {
            return _context.Author.Any(e => e.Id == id);
        }
    }

    public class AuthorCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }
    }
}

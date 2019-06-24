using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.api.Data;
using library.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace library.api.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }
        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBook(Guid authorId)
        {
            var books = await _context.Book
                .Where(b => b.AuthorId == authorId)
                .OrderBy(b => b.Title).ToListAsync();

            if (books == null)
                return NotFound();

            var bookDtos = books.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId
            }).ToList();

            return bookDtos;
        }

        // GET: api/Books/5
        [HttpGet("{id}", Name = "GetBook")]
        public async Task<ActionResult<BookDto>> GetBook(Guid authorId, Guid id)
        {
            var book = await _context.Book
                .SingleOrDefaultAsync(b => b.AuthorId == authorId && b.Id == id);

            if (book == null)
                return NotFound();

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId
            };

            return bookDto;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(Guid authorId, [FromBody] BookCreateDto bookCreateDto)
        {
            if (bookCreateDto == null)
                return BadRequest();

            if (!AuthorExists(authorId))
                return NotFound();

            var author = await _context.Author.FindAsync(authorId); //54ms

            var book = new Book()
            {
                Title = bookCreateDto.Title,
                Description = bookCreateDto.Description
            }; //65ms

            if (author != null)
            {
                author.Books.Add(book);
                await _context.SaveChangesAsync();
            } //

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId
            };

            return CreatedAtAction("GetBook", new { authorId, id = bookDto.Id }, bookDto);
           
        }


        // PUT: api/Books/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(Guid authorId, Guid id)
        {
            var book = await _context.Book
                .FirstOrDefaultAsync(b => b.AuthorId == authorId && b.Id == id);

            if (book == null) return NotFound();

            _context.Book.Remove(book);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool AuthorExists(Guid id)
        {
            return _context.Author.Any(e => e.Id == id);
        }
    }

    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}

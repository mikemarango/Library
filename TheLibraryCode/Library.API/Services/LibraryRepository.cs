﻿using Library.API.Data;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        public LibraryRepository(LibraryContext context)
        {
            Context = context;
        }

        public LibraryContext Context { get; }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            Context.Authors.Add(author);

            // the repository fills the id (instead of using identity columns)
            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                // if there isn't an id filled out (ie: we're not upserting),
                // we should generate one
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid authorId)
        {
            return Context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            Context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            Context.Books.Remove(book);
        }

        public Author GetAuthor(Guid authorId)
        {
            return Context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return Context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return Context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return Context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return Context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public void UpdateBookForAuthor(Book book)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (Context.SaveChanges() >= 0);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.api.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

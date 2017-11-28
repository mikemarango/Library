using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public Author Author { get; set; }
        public Guid AuthorId { get; set; }
    }
}

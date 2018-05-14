using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        public string Name { get; set; }

        public virtual List<Movie> Movies { get; set; } = new List<Movie>();
    }
}

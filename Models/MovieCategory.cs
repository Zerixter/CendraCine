using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class MovieCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual Category Category { get; set; }
        public virtual Movie Movie { get; set; }
    }
}

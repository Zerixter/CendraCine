using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Theater
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public int Number { get; set; }
        [Required]
        public int Capacity { get; set; }

        public virtual List<Projection> Projections { get; set; } = new List<Projection>();
        public virtual List<Seat> Seats { get; set; } = new List<Seat>();
    }
}

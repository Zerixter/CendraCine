using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public DateTime DateReservated { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public float Price { get; set; } 

        public virtual User Owner { get; set; }
        public virtual Seat Seat { get; set; }
        public virtual Projection Projection { get; set; }
    }
}

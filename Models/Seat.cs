using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Seat
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public int SeatNumber { get; set; }
        [Required]
        public int RowNumber { get; set; }

        public virtual Theater Theater { get; set; }
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        public string Role { get; set; } = "Client";

        public virtual List<Billboard> Billboards { get; set; } = new List<Billboard>();
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual List<Movie> Movies { get; set; } = new List<Movie>();
    }
}

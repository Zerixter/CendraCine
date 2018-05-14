using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class BillboardMovieRegister
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int TicketsPurchased { get; set; } = 0;

        public virtual Movie Movie { get; set; }
        public virtual Billboard Billboard { get; set; }
    }
}

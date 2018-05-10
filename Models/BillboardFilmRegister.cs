using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class BillboardFilmRegister
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketsPurchased { get; set; } = 0;
    }
}

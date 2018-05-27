using cendracine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class GetSeatsViewModel
    {
        public string Id { get; set; }
        public int SeatNumber { get; set; }
        public int RowNumber { get; set; }
        public bool Occuped { get; set; }
        public bool Selected { get; set; }

        public virtual Theater Theater { get; set; }
        public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

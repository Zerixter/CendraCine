using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class ReservationViewModel
    {
        public DateTime DateReservated { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;
        public float Price { get; set; } = 0;
        public ProjectionViewModel Projection { get; set; }
        public TheaterViewModel Theater { get; set; }
        public SeatViewModel Seat { get; set; }
    }
}

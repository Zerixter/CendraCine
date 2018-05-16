using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class ReservationViewModel
    {
        public DateTime DateReservated { get; set; }
        public bool Status { get; set; }
        public float Price { get; set; }
        public string UserName { get; set; }
        public int SeatNumber { get; set; }
        public int RowNumber { get; set; }
        public DateTime DateProjection { get; set; }
        public string MovieName { get; set; }
        public int TheaterNumber { get; set; }
        public string UserEmail { get; set; }
    }
}

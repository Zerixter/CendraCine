using cendracine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class TheaterViewModel
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public int RowNumbers { get; set; } = 0;
        public int SeatNumbers { get; set; } = 0;
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}

using cendracine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class ProjectionViewModel
    {
        public string Id { get; set; }
        public DateTime ProjectionDate { get; set; } = DateTime.Parse("10/10/1000").Date;
        public Movie Movie { get; set; }
        public Theater Theater { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

using cendracine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class BillboardUpdateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; } = DateTime.Parse("10/10/1000").Date;
        public DateTime EndDate { get; set; } = DateTime.Parse("10/10/1000").Date;
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}

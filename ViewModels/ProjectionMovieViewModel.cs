using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class ProjectionMovieViewModel
    {
        public DateTime BeginDate { get; set; } = DateTime.Parse("10/10/1000");
        public DateTime EndDate { get; set; } = DateTime.Parse("10/10/1000");
        public MovieViewModel Movie { get; set; }
    }
}

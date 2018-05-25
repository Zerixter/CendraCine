using cendracine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class MovieViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Trailer { get; set; }
        public int RecommendedAge { get; set; }
        public string Cover { get; set; }
        public List<Category> Categories = new List<Category>();
    }
}

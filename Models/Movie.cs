using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Movie
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Synopsis { get; set; }
        [MaxLength(1000)]
        public string Trailer { get; set; }
        [MaxLength(1000)]
        public string Cover { get; set; }
        [Required]
        public int RecommendedAge { get; set; }

        public virtual User Owner { get; set; }
        public virtual List<BillboardMovieRegister> BillboardMovieRegister { get; set; } = new List<BillboardMovieRegister>();
        public virtual List<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
        public virtual List<Projection> Projections { get; set; } = new List<Projection>();
    }
}

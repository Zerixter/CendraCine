using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Film
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Trailer { get; set; }
        public int RecommendedAge { get; set; }
    }
}

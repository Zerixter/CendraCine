using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class Billboard
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        [Required]
        public DateTime BeginDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;


        public virtual List<BillboardMovieRegister> BillboardMovieRegister { get; set; } = new List<BillboardMovieRegister>();
        public virtual User Owner { get; set; }
    }
}

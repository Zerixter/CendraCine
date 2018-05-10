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
        public Guid Id;
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

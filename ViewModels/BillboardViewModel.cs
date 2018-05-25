using cendracine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class BillboardViewModel
    {
        public BillboardViewModel(string name, DateTime beginDate, DateTime endDate)
        {
            Name = name;
            BeginDate = beginDate;
            EndDate = endDate;
        }

        public string Id { get; set; }
        [Required(ErrorMessage = "No s'ha introduit cap nom")]
        public string Name { get; set; }
        [Required(ErrorMessage = "No s'ha introduit una data inicial")]
        public DateTime BeginDate { get; set; }
        [Required(ErrorMessage = "No s'ha introduit una data final")]
        public DateTime EndDate { get; set; }

        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}

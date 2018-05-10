using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        
        public string IdentityId { get; set; }
        public virtual UserIdentity Identity { get; set; }
    }
}

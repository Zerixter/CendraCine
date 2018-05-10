using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Models
{
    public class UserIdentity : IdentityUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}

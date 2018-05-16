using cendracine.Data;
using cendracine.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel(string _Name, string _Email, string _Password)
        {
            Name = _Name;
            Email = _Email;
            Password = _Password;
        }

        [Required(ErrorMessage = "No s'ha introduit cap nom"), MaxLength(200)]
        public string Name { get; set; }
        [Required(ErrorMessage = "No s'ha introduit cap correu electrònic"), EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "No s'ha introduit cap contrasenya"), RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")]
        public string Password { get; set; }
    }
}

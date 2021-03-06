﻿using cendracine.Data;
using cendracine.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class CredentialsViewModel
    {
        public CredentialsViewModel(string _Email, string _Password)
        {
            Email = _Email;
            Password = _Password;
        }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public static List<object> ValidateCredentials(DbHandler db, CredentialsViewModel model)
        {
            List<object> Errors = new List<object>();
            if (model.Email is null || model.Password is null)
            {
                if (model.Email is null)
                {
                    Errors.Add(Message.GetMessage("No s'ha introduit cap compte d'usuari."));
                }
                if (model.Password is null)
                {
                    Errors.Add(Message.GetMessage("No s'ha introduit cap contrasenya."));
                }
            }
            return Errors;
        }
    }
}

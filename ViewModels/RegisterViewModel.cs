using cendracine.Data;
using cendracine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cendracine.ViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public static List<object> ValidateRegister(DbHandler db, RegisterViewModel model)
        {
            List<object> Errors = new List<object>();
            try
            {
                string regexPassword = "^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,}";
                Regex regex = new Regex(regexPassword);
                if (!regex.IsMatch(model.Password))
                {
                    Errors.Add(Message.GetMessage("La contrasenya introduida no és correcte: El format ha de ser el següent, ha de tenir mínim un numero, una lletra minúscula i una majuscula i ha de tenir una longitura de mínim 8 carácters"));
                } else
                {
                    var UserCheck = db.Users.Where(x => x.UserName == model.Email).FirstOrDefault();
                    if (UserCheck != null)
                    {
                        Errors.Add(Message.GetMessage("Un usuari amb amb aquest correu electrònic ja existeix."));
                    }
                }
            } catch (Exception)
            {
                Errors.Add(Message.GetMessage("La contrasenya introduida no és correcte: El format ha de ser el següent, ha de tenir mínim un numero, una lletra minúscula i una majuscula i ha de tenir una longitura de mínim 8 carácters"));
            }

            try
            {
                string regexMail = @"^[a-z0-9._%+-]+@[a-z0-9.-]+[^\.]\.[a-z]{2,3}$";
                Regex regex = new Regex(regexMail);
                if (!regex.IsMatch(model.Email))
                {
                    Errors.Add(Message.GetMessage("El correu electrònic introduit no és correcte."));
                }
            } catch (Exception)
            {
                Errors.Add(Message.GetMessage("El correu electrònic introduit no és correcte."));
            }

            if (model.Name is null)
            {
                Errors.Add(Message.GetMessage("El camp del nom s'ha deixat buit, s'ha de posar un nom."));
            }

            if (model.Email is null)
            {
                Errors.Add(Message.GetMessage("El camp del cognom s'ha deixat buit, s'ha de posar un cognom."));
            }
            return Errors;
        }
    }
}

using cendracine.Data;
using cendracine.Helpers;
using cendracine.Models;
using cendracine.Properties;
using cendracine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly DbHandler dbHandler;

        public AccountController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult Get()
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest(Message.GetMessage("No hi ha cap usuari loguejat"));

            var token = LoginUser(user);
            return Ok(token);
        }

        [HttpGet("role")]
        public ActionResult GetRole()
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest();

            return Ok(user.Role);
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public ActionResult Get(Guid id)
        {
            User user = dbHandler.Users.FirstOrDefault(x => x.Id == id);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        public User GetUser(string email, string password)
        {
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            return user;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterViewModel model)
        {
            User user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            try
            {
                dbHandler.Users.Add(user);
                dbHandler.SaveChanges();
                var token = LoginUser(user);
                UserViewModel userLogged = new UserViewModel
                {
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    Token = token
                };

                return Ok(userLogged);
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al registrar el usuari"));
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] CredentialsViewModel model)
        {
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
            if (user is null)
                return BadRequest();
            var token = LoginUser(user);
            UserViewModel userLogged = new UserViewModel
            {
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Token = token
            };
            return Ok(userLogged);
        }

        [HttpPut]
        public ActionResult Update([FromBody] User user)
        {
            User UserToModify = dbHandler.Users.FirstOrDefault(x => x.Id == user.Id);
            if (UserToModify is null)
                return BadRequest(Message.GetMessage("Aquest usuari no existeix"));

            if (user.Name.Length > 0)
                UserToModify.Name = user.Name;
            if (user.Password.Length > 0)
                UserToModify.Password = user.Password;

            try
            {
                dbHandler.Users.Update(UserToModify);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al actualitzar les dades del usuari"));
            }

            return Ok();
        }

        [HttpDelete]
        public ActionResult Delete([FromBody] User user)
        {
            User UserToDelete = dbHandler.Users.FirstOrDefault(x => x.Id == user.Id);
            if (UserToDelete is null)
                return BadRequest(Message.GetMessage("Aquest usuari no existeix"));

            try
            {
                dbHandler.Users.Remove(UserToDelete);
                dbHandler.SaveChanges();
                return Ok();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al intentar esborrar l'usuari"));
            }
        }

        private string LoginUser(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Resources.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: Resources.Domain,
                audience: Resources.Domain,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
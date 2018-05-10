using cendracine.Data;
using cendracine.Models;
using cendracine.Properties;
using cendracine.ViewModels;
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
        
        public AccountController(DbHandler _dbHandler)
        {
            dbHandler = _dbHandler;
        }

        [HttpGet]
        public ActionResult Get()
        {
            string Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            if (Email is null)
                return BadRequest("No user found in the database");


            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(Guid id)
        {
            User user = dbHandler.Users.FirstOrDefault(x => x.Id == id);

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
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
                    return Ok(token);
                } catch (Exception)
                {
                    return BadRequest("Error al registrar el usuari");
                }
            }
            return BadRequest();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] CredentialsViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = dbHandler.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (user is null)
                    return BadRequest();
                var token = LoginUser(user);
                return Ok(token);
            }
            return BadRequest();
        }

        private string LoginUser(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(user.Role, "")
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
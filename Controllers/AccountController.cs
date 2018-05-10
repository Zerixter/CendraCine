using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Helpers;
using cendracine.Models;
using cendracine.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly DbHandler dbHandler;
        private readonly UserManager<UserIdentity> userManager;
        private readonly IJwtFactory jwtFactory;
        private readonly JwtIssuerOptions jwtOptions;
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

        public AccountController(DbHandler _dbHandler, UserManager<UserIdentity> _userManager, IJwtFactory _jwtFactory, IOptions<JwtIssuerOptions> _jwtOptions)
        {
            dbHandler = _dbHandler;
            userManager = _userManager;
            jwtFactory = _jwtFactory;
            jwtOptions = _jwtOptions.Value;
        }

        [HttpPost("register")]
        public IActionResult CreateUser([FromBody] RegisterViewModel model)
        {
            List<object> Errors = RegisterViewModel.ValidateRegister(dbHandler, model);

            if (Errors.Count > 0)
            {
                return BadRequest(Errors);
            }

            try
            {
                UserIdentity userIdentity = new UserIdentity
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email
                };

                User user = new User
                {
                    Id = Guid.NewGuid(),
                    Identity = userIdentity
                };

                var result = userManager.CreateAsync(userIdentity, model.Password).GetAwaiter().GetResult();

                dbHandler.DbUsers.Add(user);
                dbHandler.SaveChanges();

                var identity = GetClaimsIdentity(model.Email, model.Password).GetAwaiter().GetResult();
                var token = Tokens.GenerateJwt(user.Id.ToString(), identity, jwtFactory, model.Email, jwtOptions, jsonSerializerSettings).GetAwaiter().GetResult();
                return new OkObjectResult(token);
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] CredentialsViewModel model)
        {
            List<object> Errors = CredentialsViewModel.ValidateCredentials(dbHandler, model);

            if (Errors.Count > 0)
            {
                return BadRequest(Errors);
            }

            UserIdentity userIdentity = null;
            User user = null;
            try
            {
                var identity = GetClaimsIdentity(model.Email, model.Password).GetAwaiter().GetResult();
                userIdentity = dbHandler.Users.Where(x => x.UserName == model.Email).FirstOrDefault();

                if (identity is null)
                {
                    if (userIdentity is null)
                    {
                        Errors.Add(Message.GetMessage("El compte d'usuari introduit és incorrecte"));
                    } else
                    {
                        Errors.Add(Message.GetMessage("La contrasenya introduida no és correcte"));
                    }
                    return BadRequest(Errors);
                }
                user = dbHandler.DbUsers.Where(x => x.IdentityId == userIdentity.Id).FirstOrDefault();

                string user_id = "";
                if (user != null)
                {
                    user_id = user.Id.ToString();
                }

                var token = Tokens.GenerateJwt(user_id, identity, jwtFactory, model.Email, jwtOptions, jsonSerializerSettings).GetAwaiter().GetResult();
                return new OkObjectResult(token);
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            var userToVerify = await userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            if (await userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
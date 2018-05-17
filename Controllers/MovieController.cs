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
using Microsoft.AspNetCore.Mvc;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/movie")]
    public class MovieController : Controller
    {
        private readonly DbHandler dbHandler;

        public MovieController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult Get()
        {
            List<Movie> Movies = dbHandler.Movies.ToList();
            return Ok(Movies);
        }

        [HttpPost]
        public ActionResult Create([FromBody] MovieViewModel model)
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            if (Email is null)
                return BadRequest(Message.GetMessage("Token invalido"));
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest(Message.GetMessage("Usuari no connectado"));

            Movie movie = new Movie
            {
                Name = model.Name,
                RecommendedAge = model.RecommendedAge,
                Synopsis = model.Synopsis,
                Trailer = model.Trailer,
                Cover = model.Cover,
                Owner = user
            };
            
            try
            {
                dbHandler.Movies.Add(movie);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Hi ha hagut un error al intentar afegir la película"));
            }
            return Ok();
        }
    }
}
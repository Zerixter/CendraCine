using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public ActionResult Create([FromBody] MovieViewModel model)
        {
            Movie movie = new Movie
            {
                Name = model.Name,
                RecommendedAge = model.RecommendedAge,
                Synopsis = model.Synopsis,
                Trailer = model.Trailer
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

        /*[HttpPut]
        public ActionResult Update([FromBody] MovieViewModel model)
        {
        }*/
    }
}
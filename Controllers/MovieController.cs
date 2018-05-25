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
using Microsoft.EntityFrameworkCore;

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
            List<Movie> Movies = dbHandler.Movies.Include(x => x.MovieCategories).ToList();
            return Ok(Movies);
        }

        [HttpGet("{id}")]
        public ActionResult GetMovie(string id)
        {
            Movie movie = dbHandler.Movies.Include(x => x.MovieCategories).FirstOrDefault(x => x.Id.ToString() == id);
            return Ok(movie);
        }

        [HttpPost]
        public ActionResult Create([FromBody] MovieViewModel model)
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            if (Email is null)
                return BadRequest(Message.GetMessage("Token invalido"));
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            //User user = dbHandler.Users.FirstOrDefault(x => x.Email == "hamza@cendracine.com");
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
                if (model.Categories.Count > 0)
                {
                    foreach (Category category in model.Categories)
                    {
                        Category is_in_db = dbHandler.Categories.FirstOrDefault(x => x.Id == category.Id);
                        if (is_in_db is null)
                            continue;
                        MovieCategory movieCategory = new MovieCategory
                        {
                            Category = is_in_db,
                            Movie = movie
                        };
                        movie.MovieCategories.Add(movieCategory);
                    }
                }
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Hi ha hagut un error al intentar afegir la película"));
            }
            return Ok();
        }

        [HttpPut]
        public ActionResult UpdateMovie([FromBody] MovieViewModel model)
        {
            Movie movieToSearch = dbHandler.Movies.Include(x => x.MovieCategories).FirstOrDefault(x => x.Id.ToString() == model.Id);
            if (movieToSearch is null)
                return BadRequest();

            movieToSearch.Cover = (model.Cover != movieToSearch.Cover && model.Cover.Length > 0) ? model.Cover : movieToSearch.Cover;
            movieToSearch.Name = (model.Name != movieToSearch.Name && model.Name.Length > 0) ? model.Name : movieToSearch.Name;
            movieToSearch.RecommendedAge = (model.RecommendedAge != movieToSearch.RecommendedAge && model.RecommendedAge != 0) ? model.RecommendedAge : movieToSearch.RecommendedAge;
            movieToSearch.Synopsis = (model.Synopsis != movieToSearch.Synopsis && model.Synopsis.Length > 0) ? model.Synopsis : movieToSearch.Synopsis;
            movieToSearch.Trailer = (model.Trailer != movieToSearch.Trailer && model.Trailer.Length > 0) ? model.Trailer : movieToSearch.Trailer;

            try
            {
                dbHandler.Movies.Update(movieToSearch);
                List<Category> CategoriesToIgnore = new List<Category>();
                // Comproba si les categories que ja te asignades se l'has ha pasat el usuari en la llista, si no les esborra
                foreach (MovieCategory mc in movieToSearch.MovieCategories)
                {
                    MovieCategory mc_db = dbHandler.MovieCategories.Include(x => x.Category).FirstOrDefault(x => x.Id == mc.Id);
                    if (mc_db is null || model.Categories.Contains(mc_db.Category))
                    {
                        CategoriesToIgnore.Add(mc_db.Category);
                        continue;
                    }
                    dbHandler.MovieCategories.Remove(mc_db);
                }
                // Comproba que no es repeteixin categories si no hi ha de repetides afageix les noves afegides en la llista
                foreach (Category category in model.Categories)
                {
                    Category is_in_db = dbHandler.Categories.FirstOrDefault(x => x.Id == category.Id);
                    if (is_in_db is null || CategoriesToIgnore.Contains(is_in_db))
                        continue;
                    MovieCategory movieCategory = new MovieCategory
                    {
                        Category = is_in_db,
                        Movie = movieToSearch
                    };
                    movieToSearch.MovieCategories.Add(movieCategory);
                }
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMovie([FromRoute] string id)
        {
            Movie movieToDelete = dbHandler.Movies.FirstOrDefault(x => x.Id.ToString() == id);
            if (movieToDelete is null)
                return BadRequest();

            try
            {
                List<MovieCategory> movieCategories = dbHandler.MovieCategories.Include(x => x.Movie).Where(x => x.Movie.Id == movieToDelete.Id).ToList();
                dbHandler.MovieCategories.RemoveRange(movieCategories);
                dbHandler.Movies.Remove(movieToDelete);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
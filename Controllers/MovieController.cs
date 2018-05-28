using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Helpers;
using cendracine.Models;
using cendracine.Properties;
using cendracine.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                return BadRequest();
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest();

            try
            {
                float Rating = GetRating(model);

                Movie movie = new Movie
                {
                    Name = model.Name,
                    RecommendedAge = model.RecommendedAge,
                    Synopsis = model.Synopsis,
                    Trailer = model.Trailer,
                    Cover = model.Cover,
                    Owner = user,
                    Rating = Rating
                };
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

            if (model.Cover != movieToSearch.Cover && model.Cover.Length > 0)
            {
                DeleteImage(movieToSearch.Cover);
                movieToSearch.Cover = model.Cover;
            }
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
                DeleteImage(movieToDelete.Cover);
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        public float GetRating(MovieViewModel model)
        {
            string ApiKey = Resources.ImdbAPI;
            string[] Name = model.Name.Split(" ");
            string MovieName = "";
            string s = "";
            foreach (var n in Name)
            {
                MovieName += s + n;
                s = "-";
            }

            MovieName = MovieName.ToLower();

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://www.omdbapi.com/")
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"?apikey={ApiKey}&t={MovieName}");

                var x = httpClient.SendAsync(request).GetAwaiter().GetResult();
                string json_txt = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                ImdbViewModel imdb = JsonConvert.DeserializeObject<ImdbViewModel>(json_txt);
                return float.Parse(imdb.imdbRating);
            } catch (Exception)
            {
                return 0;
            }
        }

        public void DeleteImage(string Cover)
        {
            Cover = Cover.Replace("http://localhost:5000/assets/", "");
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), @"wwwroot/assets",
                        Cover);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
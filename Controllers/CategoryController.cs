using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Models;
using cendracine.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/category")]
    public class CategoryController : Controller
    {
        private readonly DbHandler dbHandler;

        public CategoryController()
        {
            dbHandler = new DbHandler();
        }

        // GET: api/Category
        [HttpGet]
        public ActionResult Get()
        {
            List<Category> categories = new List<Category>();
            try
            {
                categories = dbHandler.Categories.ToList();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}", Name = "GetCategory")]
        public ActionResult GetCategory(string id)
        {
            Category category = dbHandler.Categories.FirstOrDefault(x => x.Id.ToString() == id);
            if (category is null)
                return BadRequest();
            return Ok(category);
        }

        [HttpGet("moviecategory/{id}")]
        public ActionResult GetMovieCategory(string id)
        {
            List<MovieCategory> movieCategory = dbHandler.MovieCategories.Include(x => x.Category).Include(x => x.Movie).Where(x => x.Movie.Id.ToString() == id).ToList();
            if (movieCategory is null)
                return BadRequest();
            return Ok(movieCategory);
        }
        
        // POST: api/Category
        [HttpPost]
        public ActionResult CreateCategory([FromBody] CategoryViewModel model)
        {
            Category category = new Category
            {
                Name = model.Name
            };

            try
            {
                dbHandler.Categories.Add(category);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        // PUT: api/Category/5
        [HttpPut]
        public ActionResult UpdateCategory([FromBody] CategoryViewModel model)
        {
            Category category = dbHandler.Categories.FirstOrDefault(x => x.Id.ToString() == model.Id);
            if (category is null)
                return BadRequest();

            category.Name = model.Name;

            try
            {
                dbHandler.Categories.Update(category);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(string id)
        {
            Category category = dbHandler.Categories.FirstOrDefault(x => x.Id.ToString() == id);
            if (category is null)
                return BadRequest();

            try
            {
                dbHandler.Categories.Remove(category);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}

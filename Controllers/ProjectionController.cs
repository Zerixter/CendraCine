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
    [Route("api/projection")]
    public class ProjectionController : Controller
    {
        private readonly DbHandler dbHandler;

        public ProjectionController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult GetProjections()
        {
            List<Projection> projections = dbHandler.Projections.ToList();
            return Ok(projections);
        }

        [HttpGet("{id}")]
        public ActionResult GetProjection([FromRoute] string id)
        {
            Projection projection = dbHandler.Projections.FirstOrDefault(x => x.Id.ToString() == id);
            if (projection is null)
                return BadRequest();

            return Ok(projection);
        }

        [HttpPost]
        public ActionResult CreateProjection([FromBody] ProjectionViewModel model)
        {
            Movie movie = dbHandler.Movies.FirstOrDefault(x => x.Id == model.Movie.Id);
            if (movie is null)
                return BadRequest();
            Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id == model.Theater.Id);
            if (theater is null)
                return BadRequest();

            try
            {
                Projection projection = new Projection
                {
                    ProjectionDate = model.ProjectionDate,
                    Movie = movie,
                    Theater = model.Theater
                };
                dbHandler.Projections.Add(projection);
                dbHandler.SaveChanges();
                return Ok();
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public ActionResult UpdateProjection([FromBody] ProjectionViewModel model)
        {
            Projection projection = dbHandler.Projections.Include(x => x.Movie).Include(x => x.Theater).FirstOrDefault(x => x.Id.ToString() == model.Id);
            if (projection is null)
                return BadRequest();

            if (projection.Theater.Id != model.Theater.Id)
            {
                Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id == model.Theater.Id);
                projection.Theater = theater ?? projection.Theater;
            }
            if (projection.Movie.Id != model.Movie.Id)
            {
                Movie movie = dbHandler.Movies.FirstOrDefault(x => x.Id == model.Movie.Id);
                projection.Movie = movie ?? projection.Movie;
            }
            projection.ProjectionDate = (projection.ProjectionDate != model.ProjectionDate && model.ProjectionDate != DateTime.Parse("10/10/1000").Date) ? model.ProjectionDate : projection.ProjectionDate;
            
            try
            {
                dbHandler.Projections.Update(projection);
                dbHandler.SaveChanges();
                return Ok();
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProjection([FromRoute] string id)
        {
            Projection projection = dbHandler.Projections.FirstOrDefault(x => x.Id.ToString() == id);
            if (projection is null)
                return BadRequest();

            try
            {
                List<Reservation> reservations = dbHandler.Reservations.Include(x => x.Projection).Where(x => x.Projection.Id == projection.Id).ToList();
                dbHandler.Reservations.RemoveRange(reservations);
                dbHandler.Projections.Remove(projection);
                dbHandler.SaveChanges();
                return Ok();
            } catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
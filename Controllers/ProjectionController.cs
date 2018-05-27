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
            List<Projection> projections = dbHandler.Projections.Include(x => x.Movie).Include(x => x.Theater).ToList();
            return Ok(projections);
        }

        [HttpGet("{id}")]
        public ActionResult GetProjection([FromRoute] string id)
        {
            Projection projection = dbHandler.Projections.Include(x => x.Movie).Include(x => x.Theater).FirstOrDefault(x => x.Id.ToString() == id);
            if (projection is null)
                return BadRequest();

            return Ok(projection);
        }

        [HttpPost("movie")]
        public ActionResult GetProjectionsOfMovie([FromBody] ProjectionMovieViewModel model)
        {
            try
            {
                Movie movie = dbHandler.Movies.FirstOrDefault(x => x.Id.ToString() == model.Movie.Id);
                if (movie is null)
                    return BadRequest();
                if (model.EndDate == DateTime.Parse("10/10/1000") || model.BeginDate == DateTime.Parse("10/10/1000"))
                    return BadRequest();

                List<Projection> projections = 
                    dbHandler.
                    Projections.
                    Include(x => x.Reservations).
                    Include(x => x.Movie).
                    Include(x => x.Theater).
                    Where(
                        x => 
                        x.ProjectionDate >= model.BeginDate &&
                        x.ProjectionDate <= model.EndDate &&
                        x.Movie.Id == movie.Id &&
                        x.Reservations.Count < x.Theater.Capacity
                    ).ToList();

                return Ok(projections);
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("seats/{id}")]
        public ActionResult GetSeatsFromProjection([FromRoute] string id)
        {
            try
            {
                Projection projection = dbHandler.Projections.Include(x => x.Theater).FirstOrDefault(x => x.Id.ToString() == id);
                if (projection is null)
                    return BadRequest();
                //List<Seat> Seats = dbHandler.Seats.Include(x => x.Theater).Include(x => x.Reservations).Where(x => x.Theater.Id == projection.Theater.Id).ToList();
                IEnumerable<GetSeatsViewModel> Seats = dbHandler.Seats.Join(dbHandler.Theaters, x => x.Theater.Id, s => s.Id, (x, s) => new GetSeatsViewModel
                {
                    Id = x.Id.ToString(),
                    Theater = x.Theater,
                    RowNumber = x.RowNumber,
                    SeatNumber = x.SeatNumber,
                    Reservations = x.Reservations,
                    Occuped = false
                }).Where(x => x.Theater.Id == projection.Theater.Id);

                return Ok(Seats);
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("reservations/{id}")]
        public ActionResult GetReservationsFromProjections([FromRoute] string id)
        {
            try
            {
                Projection projection = dbHandler.Projections.FirstOrDefault(x => x.Id.ToString() == id);
                if (projection is null)
                    return BadRequest();
                List<Reservation> reservations = dbHandler.Reservations.Include(x => x.Projection).Where(x => x.Projection.Id == projection.Id).ToList();
                return Ok(reservations);
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult CreateProjection([FromBody] ProjectionViewModel model)
        {
            try
            {
                Movie movie = dbHandler.Movies.FirstOrDefault(x => x.Id.ToString() == model.Movie.Id);
                if (movie is null)
                    return BadRequest();
                Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id.ToString() == model.Theater.Id);
                if (theater is null)
                    return BadRequest();
                Projection projection = new Projection
                {
                    ProjectionDate = model.ProjectionDate,
                    Movie = movie,
                    Theater = theater
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

            if (projection.Theater.Id.ToString() != model.Theater.Id)
            {
                Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id.ToString() == model.Theater.Id);
                projection.Theater = theater ?? projection.Theater;
            }
            if (projection.Movie.Id.ToString() != model.Movie.Id)
            {
                Movie movie = dbHandler.Movies.FirstOrDefault(x => x.Id.ToString() == model.Movie.Id);
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
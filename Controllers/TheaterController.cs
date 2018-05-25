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
    [Route("api/theater")]
    public class TheaterController : Controller
    {
        private readonly DbHandler dbHandler;

        public TheaterController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult GetTheaters()
        {
            List<Theater> Theaters = dbHandler.Theaters.ToList();
            return Ok(Theaters);
        }

        [HttpGet("{id}")]
        public ActionResult GetTheater([FromRoute] string id)
        {
            Theater theater = dbHandler.Theaters.Include(x => x.Seats).FirstOrDefault(x => x.Id.ToString() == id);
            if (theater is null)
                return BadRequest();
            return Ok(theater);
        }

        [HttpPost]
        public ActionResult CreateTheater([FromBody] TheaterViewModel model)
        {

            Theater theater = new Theater
            {
                Number = model.Number,
                Capacity = model.Capacity,
                Seats = model.Seats,
                RowNumbers = model.RowNumbers,
                SeatNumbers = model.SeatNumbers
            };

            try
            {
                dbHandler.Theaters.Add(theater);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPut]
        public ActionResult UpdateTheater([FromBody] TheaterViewModel model)
        {
            Theater theater = dbHandler.Theaters.Include(x => x.Seats).FirstOrDefault(x => x.Id.ToString() == model.Id);
            if (theater is null)
                return BadRequest();

            theater.Number = (theater.Number != model.Number && model.Number != 0) ? model.Number : theater.Number;
            theater.RowNumbers = (theater.RowNumbers != model.RowNumbers && model.RowNumbers != 0) ? model.RowNumbers : theater.RowNumbers;
            theater.SeatNumbers = (theater.SeatNumbers != model.SeatNumbers && model.SeatNumbers != 0) ? model.SeatNumbers : theater.SeatNumbers;
            theater.Capacity = (theater.Capacity != model.Capacity && model.Capacity != 0) ? model.Capacity : theater.Capacity;

            try
            {
                dbHandler.Theaters.Update(theater);
                if ((model.RowNumbers != 0 && model.RowNumbers != theater.RowNumbers) || (model.SeatNumbers != 0 && model.SeatNumbers != theater.SeatNumbers))
                {
                    List<Seat> seats = dbHandler.Seats.Include(x => x.Theater).Where(x => x.Theater.Id == theater.Id).ToList();
                    dbHandler.Seats.RemoveRange(seats);
                    theater.Seats.Clear();
                    theater.Seats = model.Seats;
                }
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTheater([FromRoute] string id)
        {
            Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id.ToString() == id);
            if (theater is null)
                return BadRequest();

            List<Seat> seats = dbHandler.Seats.Include(x => x.Theater).Where(x => x.Theater.Id == theater.Id).ToList();
            try
            {
                dbHandler.Seats.RemoveRange(seats);
                dbHandler.Theaters.Remove(theater);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}

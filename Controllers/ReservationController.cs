using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Helpers;
using cendracine.Models;
using cendracine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/reservation")]
    public class ReservationController : Controller
    {
        private readonly DbHandler dbHandler;

        public ReservationController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult GetReservations()
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest();
            List<Reservation> reservations = dbHandler.Reservations.Include(x => x.Projection).Include(x => x.Seat).Include(x => x.Owner).Where(x => x.Owner.Id == user.Id).ToList();
            return Ok(reservations);
        }

        [HttpPost]
        public ActionResult Create([FromBody] ReservationViewModel model)
        {
            try
            {
                string Email = User.FindFirstValue(ClaimTypes.Email);
                User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
                if (user is null)
                    return BadRequest();
                Projection projection = dbHandler.Projections.FirstOrDefault(x => x.Id.ToString() == model.Projection.Id);
                if (projection is null)
                    return BadRequest();
                Theater theater = dbHandler.Theaters.FirstOrDefault(x => x.Id.ToString() == model.Theater.Id);
                if (theater is null)
                    return BadRequest();
                Seat seat = dbHandler.Seats.FirstOrDefault(x => x.Id.ToString() == model.Seat.Id);
                if (seat is null)
                    return BadRequest();

                Reservation reservation = new Reservation
                {
                    DateReservated = model.DateReservated,
                    Owner = user,
                    Price = model.Price,
                    Projection = projection,
                    Seat = seat,
                    Status = true
                };

                dbHandler.Reservations.Add(reservation);
                dbHandler.SaveChanges();
                return Ok();
            } catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
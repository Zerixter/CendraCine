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

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/reservation")]
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly DbHandler dbHandler;

        public ReservationController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public ActionResult Get()
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            if (Email is null)
                return BadRequest(Message.GetMessage("Token invalido"));
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest(Message.GetMessage("Usuari no connectado"));
            IEnumerable<ReservationViewModel> Reservations = null;
            try
            {
                Reservations = dbHandler.Reservations.Join(dbHandler.Users, x => x.Owner.Id, us => us.Id, (x, us) => new ReservationViewModel
                {
                    DateProjection = x.Projection.ProjectionDate,
                    DateReservated = x.DateReservated,
                    MovieName = x.Projection.Movie.Name,
                    Price = x.Price,
                    RowNumber = x.Seat.RowNumber,
                    SeatNumber = x.Seat.SeatNumber,
                    Status = x.Status,
                    TheaterNumber = x.Projection.Theater.Number,
                    UserName = x.Owner.Name,
                    UserEmail = Email
                }).Where(x => x.UserEmail == Email).ToArray();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al intentar visualitzar les reserves"));
            }
            return Ok(Reservations);
        }

        [HttpPost]
        public ActionResult Create([FromBody] ReservationViewModel model)
        {
            return Ok();
        }
    }
}
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

            List<Reservation> Reservations = dbHandler.Reservations.Where(x => x.Owner == user).ToList();
            /*List<Reservation> r = dbHandler.Reservations.Join(dbHandler.Users, x => x.Owner.Id, us => us.Id, (x, us) => new ReservationViewModel
            {
                MovieName = x.Projection.Movie.Name
            });*/
            return Ok(Reservations);
        }

        [HttpPost]
        public ActionResult Create()
        {

            return Ok();
        }
    }
}
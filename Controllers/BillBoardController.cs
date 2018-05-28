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
    [Route("api/billboard")]
    public class BillBoardController : Controller
    {
        private readonly DbHandler dbHandler;

        public BillBoardController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet]
        public IActionResult GetBillBoards()
        {
            List<Billboard> Billboards = dbHandler.Billboards.OrderByDescending(x => x.DateCreated).ToList();
            return Ok(Billboards);
        }

        [HttpGet("{id}")]
        public ActionResult GetBillboardId([FromRoute] string id)
        {
            Billboard billboard = dbHandler.Billboards.Include(x => x.BillboardMovieRegister).FirstOrDefault(x => x.Id.ToString() == id);
            if (billboard is null)
                return BadRequest();

            return Ok(billboard);
        }

        [HttpGet("actual")]
        public IActionResult GetBillBoard()
        {
            List<Billboard> billBoards = dbHandler.Billboards.Include(x => x.BillboardMovieRegister).Where(x => x.BeginDate <= DateTime.Now && x.EndDate >= DateTime.Now).ToList();
            List<BillboardMovieRegister> billboardMovieRegisters = new List<BillboardMovieRegister>();

            DateTime DefaultDate = DateTime.Parse("10/10/1000").Date;

            DateTime beginDate = DefaultDate;
            DateTime endDate = DefaultDate;
            User owner = null;

            foreach (Billboard bb in billBoards)
            {
                if (owner is null)
                    owner = bb.Owner;
                if (beginDate == DefaultDate || beginDate > bb.BeginDate)
                    beginDate = bb.BeginDate;

                if (endDate == DefaultDate || endDate < bb.EndDate)
                    endDate = bb.EndDate;

                foreach (BillboardMovieRegister bbmr in bb.BillboardMovieRegister)
                {
                    BillboardMovieRegister bbmr_db = dbHandler.BillboardMovieRegisters.Include(x => x.Movie).FirstOrDefault(x => x.Id == bbmr.Id);
                    if (bbmr_db is null)
                        continue;
                    billboardMovieRegisters.Add(bbmr_db);
                }
            }

            Billboard Billboard = new Billboard
            {
                BeginDate = beginDate,
                EndDate = endDate,
                BillboardMovieRegister = billboardMovieRegisters,
                Owner = owner
            };

            return Ok(Billboard);
        }

        [HttpPost]
        public IActionResult CreateBillBoard([FromBody] BillboardViewModel model)
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            if (Email is null)
                return BadRequest();
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest();
            try
            {
                Billboard billboard = new Billboard
                {
                    Name = model.Name,
                    BeginDate = model.BeginDate,
                    EndDate = model.EndDate,
                    Owner = user
                };
                dbHandler.Billboards.Add(billboard);
                if (model.Movies.Count > 0)
                {
                    foreach (Movie movie in model.Movies)
                    {
                        Movie movie_db = dbHandler.Movies.FirstOrDefault(x => x.Id == movie.Id);
                        if (movie_db is null)
                            continue;

                        BillboardMovieRegister billboardMovieRegister = new BillboardMovieRegister
                        {
                            Movie = movie_db,
                            Billboard = billboard
                        };
                        dbHandler.BillboardMovieRegisters.Add(billboardMovieRegister);
                    }
                }
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al intentar crear una cartellera"));
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateBillBoard([FromBody] BillboardUpdateViewModel model)
        {
            Billboard billBoardToUpdate = dbHandler.Billboards.Include(x => x.BillboardMovieRegister).FirstOrDefault(x => x.Id.ToString() == model.Id);
            DateTime DefaultDate = DateTime.Parse("10/10/1000").Date;

            billBoardToUpdate.Name = (model.Name.Length > 0) ? model.Name : billBoardToUpdate.Name;
            billBoardToUpdate.BeginDate = (model.BeginDate != DefaultDate) ? model.BeginDate : billBoardToUpdate.BeginDate;
            billBoardToUpdate.EndDate = (model.EndDate != DefaultDate) ? model.EndDate : billBoardToUpdate.EndDate;

            try
            {
                dbHandler.Billboards.Update(billBoardToUpdate);
                List<Movie> moviesToIgnore = new List<Movie>();
                foreach (BillboardMovieRegister bbmr in billBoardToUpdate.BillboardMovieRegister)
                {
                    BillboardMovieRegister bbmr_db = dbHandler.BillboardMovieRegisters.Include(x => x.Movie).FirstOrDefault(x => x.Id == bbmr.Id);
                    if (bbmr_db is null || model.Movies.Contains(bbmr_db.Movie))
                    {
                        moviesToIgnore.Add(bbmr_db.Movie);
                        continue;
                    }
                    dbHandler.BillboardMovieRegisters.Remove(bbmr_db);
                }
                foreach (Movie m in model.Movies)
                {
                    Movie is_in_db = dbHandler.Movies.FirstOrDefault(x => x.Id == m.Id);
                    if (is_in_db is null || moviesToIgnore.Contains(is_in_db))
                        continue;
                    BillboardMovieRegister bbmr = new BillboardMovieRegister
                    {
                        Billboard = billBoardToUpdate,
                        Movie = is_in_db
                    };
                    billBoardToUpdate.BillboardMovieRegister.Add(bbmr);
                }
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error la intentar actualitzar la cartellera"));
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBillBoard([FromRoute] string id)
        {
            Billboard billBoardToDelete = dbHandler.Billboards.Include(x => x.BillboardMovieRegister).FirstOrDefault(x => x.Id.ToString() == id);
            if (billBoardToDelete is null)
                return NotFound();

            try
            {
                List<BillboardMovieRegister> billboardMovieRegisters = dbHandler.BillboardMovieRegisters.Include(x => x.Billboard).Where(x => x.Billboard.Id == billBoardToDelete.Id).ToList();
                dbHandler.BillboardMovieRegisters.RemoveRange(billboardMovieRegisters);
                dbHandler.Billboards.Remove(billBoardToDelete);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al intentar esborrar la cartellera"));
            }
            return Ok();
        }
    }
}
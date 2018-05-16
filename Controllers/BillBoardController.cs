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

        [HttpGet("actual")]
        public IActionResult GetBillBoard()
        {
            List<Billboard> billBoards = dbHandler.Billboards.Where(x => x.BeginDate <= DateTime.Now && x.EndDate >= DateTime.Now).ToList();
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
                    billboardMovieRegisters.Add(bbmr);
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

        [HttpPost, ValidateModel, Authorize]
        public IActionResult CreateBillBoard([FromBody] BillboardViewModel model)
        {
            string Email = User.FindFirstValue(ClaimTypes.Email);
            if (Email is null)
                return BadRequest(Message.GetMessage("Token invalido"));
            User user = dbHandler.Users.FirstOrDefault(x => x.Email == Email);
            if (user is null)
                return BadRequest(Message.GetMessage("Usuari no connectado"));

            Billboard billboard = new Billboard
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                Owner = user
            };

            try
            {
                dbHandler.Billboards.Add(billboard);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error al intentar crear una cartellera"));
            }
            return Ok();
        }

        [HttpPost, ValidateModel]
        public IActionResult CreateBillBoard([FromBody] BillboardViewModel model, User user)
        {
            Billboard billboard = new Billboard
            {
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                Owner = user
            };

            try
            {
                dbHandler.Billboards.Add(billboard);
                dbHandler.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(Message.GetMessage("Error al intentar crear una cartellera"));
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateBillBoard([FromBody] BillboardUpdateViewModel model)
        {
            Billboard billBoardToUpdate = dbHandler.Billboards.FirstOrDefault(x => x.Id.ToString() == model.Id);
            DateTime DefaultDate = DateTime.Parse("10/10/1000").Date;

            if (model.Name.Length > 0)
                billBoardToUpdate.Name = model.Name;
            if (model.BeginDate != DefaultDate)
                billBoardToUpdate.BeginDate = model.BeginDate;
            if (model.EndDate != DefaultDate)
                billBoardToUpdate.EndDate = model.EndDate;

            try
            {
                dbHandler.Billboards.Update(billBoardToUpdate);
                dbHandler.SaveChanges();
            } catch (Exception)
            {
                return BadRequest(Message.GetMessage("Error la intentar actualitzar la cartellera"));
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteBillBoard([FromBody] BillboardViewModel model)
        {
            Billboard billBoardToDelete = dbHandler.Billboards.FirstOrDefault(x => x.Id.ToString() == model.Id);
            if (billBoardToDelete is null)
                return NotFound();

            try
            {
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
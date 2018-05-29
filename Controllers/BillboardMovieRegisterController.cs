using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/billboardmovieregister")]
    public class BillboardMovieRegisterController : Controller
    {
        private readonly DbHandler dbHandler;

        public BillboardMovieRegisterController()
        {
            dbHandler = new DbHandler();
        }

        [HttpGet("billboard/{id}")]
        public ActionResult GetBMR([FromRoute] string id)
        {
            List<BillboardMovieRegister
                > billboardMovieRegister = dbHandler.BillboardMovieRegisters.Include(x => x.Billboard).Include(x => x.Movie).Where(x => x.Billboard.Id.ToString() == id).ToList();
            if (billboardMovieRegister is null)
                return BadRequest();

            return Ok(billboardMovieRegister);
        }
    }
}

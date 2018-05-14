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
    [Route("api/billboard")]
    public class BillBoardController : Controller
    {
        private readonly DbHandler dbHandler;

        public BillBoardController()
        {
            dbHandler = new DbHandler();
        }

        public IActionResult GetBillBoard()
        {
            List<Billboard> billBoard = dbHandler.Billboards.Where(x => x.BeginDate <= DateTime.Now && x.EndDate >= DateTime.Now).ToList();
            return Ok(billBoard);
        }
    }
}
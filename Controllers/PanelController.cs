using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cendracine.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/panel")]
    public class PanelController : Controller
    {
        private readonly DbHandler dbHandler;

        public PanelController()
        {
            dbHandler = new DbHandler();
        }


    }
}
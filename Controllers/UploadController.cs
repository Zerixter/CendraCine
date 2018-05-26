using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cendracine.Data;
using cendracine.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cendracine.Controllers
{
    [Produces("application/json")]
    [Route("api/upload")]
    public class UploadController : Controller
    {
        private readonly DbHandler dbHandler;

        public UploadController()
        {
            dbHandler = new DbHandler();
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "image/jpg")
            {
                return BadRequest();
            }

            try
            {
                var Name = file.FileName;

                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), @"assets/covers",
                            Name);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return Ok(path);
            } catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
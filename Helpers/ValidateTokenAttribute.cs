using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace cendracine.Helpers
{
    public class ValidateTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            string Email = context.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            if (Email is null)
                context.Result = new BadRequestObjectResult("Failed to get email addres from token");
        }
    }
}

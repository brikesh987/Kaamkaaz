using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaamkaazServices.Filters
{
    public class HawkAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //actionContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            return;
        }
    }
}

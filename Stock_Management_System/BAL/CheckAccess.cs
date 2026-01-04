using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stock_Management_System.BAL
{
   
  
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("Auth_ID") == null)
            {

                filterContext.HttpContext.Session.Clear(); // Get current path and query string
                var request = filterContext.HttpContext.Request;
                var returnTo = request.Path + request.QueryString;

                // Redirect to login with returnTo
                filterContext.Result = new RedirectResult($"/Auth/Login?returnTo={Uri.EscapeDataString(returnTo)}");
            }
        }


        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(filterContext);
        }
    }
}

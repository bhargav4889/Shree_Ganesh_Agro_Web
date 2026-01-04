using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stock_Management_System.BAL
{
   
  
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            if (session.GetString("Auth_ID") == null)
            {
                session.Clear();

                var request = filterContext.HttpContext.Request;
                var path = request.Path.Value; // e.g. "/Invoice/PurchaseInvoices" or "/"

                // Ignore root/home page for returnTo
                string? returnTo = null;
                if (!string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(path, "/Home/Index", StringComparison.OrdinalIgnoreCase))
                {
                    returnTo = path + request.QueryString;
                }

                if (!string.IsNullOrEmpty(returnTo))
                {
                    // Only add returnTo if user tried a protected page
                    filterContext.Result = new RedirectResult($"/Auth/Login?returnTo={Uri.EscapeDataString(returnTo)}");
                }
                else
                {
                    // Otherwise just go to login without returnTo
                    filterContext.Result = new RedirectResult("/Auth/Login");
                }
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

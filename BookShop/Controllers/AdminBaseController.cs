using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookShop.Controllers
{
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.Session.GetString("username");
            var role = context.HttpContext.Session.GetString("role");
            if (username == null || role != "Admin")
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                
            }
            base.OnActionExecuting(context);
        }
    }
}

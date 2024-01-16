using System.Web.Mvc;
using System.Web.Routing;

namespace StoreTanPhat.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (Session["ID_Staff"] == null)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Home" }));
            else
            {
                if (Session["Quyen"].ToString().ToLower() == "user")
                {
                    if (controllerName != "user")
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Unauthorized", controller = "Home" }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
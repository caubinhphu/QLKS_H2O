using QLKS_H2O.Areas.Admin.Models;
using QLKS_H2O.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QLKS_H2O.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        private QLKS_H2OEntities db = new QLKS_H2OEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (LoginSessionModel)Session["session"];

            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login", Area = "Admin" }));
            } else
            {
                var user = db.NHANVIENs.Find(session.username);
                if (user == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login", Area = "Admin" }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
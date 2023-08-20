using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxesDemo.Controllers
{
    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.HttpContext.Response.AddHeader("Content-Security-Policy-Report-Only",
                "script-src 'self' 'unsafe-eval' 'unsafe-inline' *.googletagmanager.com *.mxpnl.com *.negishim.com; " +
                "report-uri /Utils/CspReport/");

            filterContext.HttpContext.Response.AddHeader("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");

            base.OnActionExecuting(filterContext);
        }
    }
}
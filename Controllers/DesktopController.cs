using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxesDemo.Controllers
{
    public class DesktopController : Controller
    {
        // GET: Desktop
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Callback(string code, string state)
        {
            ViewBag.Code = code;
            return View();
        }

    }
}
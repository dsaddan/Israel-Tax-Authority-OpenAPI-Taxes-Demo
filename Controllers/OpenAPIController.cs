using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaxesDemo.BL;

namespace TaxesDemo.Controllers
{
    public class OpenAPIController : Controller
    {

        [HttpGet, AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public void Index(string code, string state)
        {

            Globals.Set("redirect_state", state);
            Globals.Set("code", code);

            Response.Redirect("/?#3");
        }

    }
}
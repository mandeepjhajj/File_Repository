using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileRepository.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        [ActionName("404")]
        public ActionResult Error404()
        {
            return View();
        }
	}
}
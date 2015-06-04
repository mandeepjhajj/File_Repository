using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileRepository.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
       {

            bool Aresult = User.IsInRole("admin");

            if(Aresult)
            {
            Session["CurrentRole"] = "admin";
            }

            bool Dresult = User.IsInRole("developer");

            if (Dresult)
            {
                Session["CurrentRole"] = "developer";
            }
            bool Uresult = User.IsInRole("naiveUser");

            if (Uresult)
            {
                Session["CurrentRole"] = "naiveUser";
            }

            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
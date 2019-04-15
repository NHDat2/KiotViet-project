using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSM_Backend.Controllers
{
    public class SignupController : Controller
    {
        // GET: Signup
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string USERNAME, string PASSWORD, string EMAIL)
        {
            return Json(200,JsonRequestBehavior.AllowGet);
        }


    }
}
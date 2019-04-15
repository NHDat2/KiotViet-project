using OSM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSM_Backend.Controllers
{
    public class OutsideController : Controller
    {
        // GET: Outside
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string USERNAME, string PASSWORD)
        {
            int code = 400;
            string message = "Thất bại";
            int levelAdmin = 0;

            if (!ModelState.IsValid)
            {
                code = 400;
                message = "Thất bại vì không biết tại sao";
                levelAdmin = -999;
                return Json(new { code, message, levelAdmin }, JsonRequestBehavior.AllowGet);
            }
            bool checkUSERNAME = false;
            DataTable dt = AccountModel.GetAll();
            var lst = dt.AsEnumerable().Select(r => r.Field<string>("username")).ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i] == USERNAME)
                {
                    checkUSERNAME = true;
                    message = "Mật khẩu sai";
                    var pass = dt.Rows[i].Field<string>("password");
                    if (pass == PASSWORD)
                    {
                        code = 200;
                        message = "Thành Công";
                        levelAdmin = dt.Rows[i].Field<int>("levelAdmin");
                    }
                }
            }
            if (checkUSERNAME == false)
                message = "Không tồn tại tên đăng nhập";

            return Json(new { code, message, levelAdmin }, JsonRequestBehavior.AllowGet);
        }
    }
}
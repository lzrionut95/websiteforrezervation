using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteForReservation;

namespace WebsiteForReservation.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Home
        public ActionResult Index()
        {
            if (Session["Email"] != null)
            {
                var user1 = db.User1.Include(u => u.Reservation);
                return View(user1.ToList());
            }
            else
            {
                return RedirectToAction("Index", "LoginController", new { area = "Login" });
            }
        }

    }
}

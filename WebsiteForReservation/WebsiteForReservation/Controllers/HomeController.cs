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

        public ActionResult Index()
        {
            if (Session["Email"] != null)
            {
                var user1 = db.User1.Include(u => u.Reservation);
                return View(user1.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login", new { area = "Login" });
            }
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult ChangePassword(string oldPassword,string newPassword,string confirmNewPassword)
        {          
            if (Session["UserId"] != null)
            {
                int usrId = Convert.ToInt32(Session["UserId"].ToString());
                User1 user=db.User1.Single(usr => usr.UserId == usrId);
                if (oldPassword != user.Password && oldPassword!= newPassword && newPassword == confirmNewPassword)
                {
                        user.Password = newPassword;
                        db.SaveChanges();
                    
                }
            }
            else
            {
                
            }
            return RedirectToAction("Settings", "Home", new { area = "Home" });
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Login", new { area = "Login" });
        }

    }
}
